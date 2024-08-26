using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Contracts.Infraestructure.FileStorage;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Contracts.Services;
using SupportDesk.Application.Models.Dtos;

namespace SupportDesk.Application.Features.UsersManagement.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UpdateUserCommandResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IPasswordService _passwordService;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateUserCommandHandler> _logger;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IFileStorageService fileStorageService,
        IPasswordService passwordService,
        IMapper mapper,
        ILogger<UpdateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _fileStorageService = fileStorageService;
        _passwordService = passwordService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UpdateUserCommandResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var response = new UpdateUserCommandResponse();

        var userToUpdate = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (userToUpdate == null)
        {
            response.Success = false;
            response.Message = UsersMessages.UserNotFound;
            return response;
        }

        // Actualizar los campos básicos
        userToUpdate.Email = request.Email;
        userToUpdate.FirstName = request.FirstName;
        userToUpdate.LastName = request.LastName;
        userToUpdate.IsAdmin = request.IsAdmin;
        userToUpdate.IsSupervisor = request.IsSupervisor;
        userToUpdate.IsActive = request.IsActive;

        // Si se proporciona una nueva contraseña, actualizar el hash de la contraseña
        if (!string.IsNullOrEmpty(request.Password))
        {
            userToUpdate.PasswordHash = _passwordService.HashPassword(request.Password);
        }

        // Si se proporciona una nueva foto, actualizar la URL de la foto
        if (request.Photo != null)
        {
            var uploadedUrls = await _fileStorageService.UploadFilesAsync(new List<IFormFile> { request.Photo }, "users", cancellationToken);
            userToUpdate.PhotoUrl = uploadedUrls.FirstOrDefault();
        }

        // Guardar los cambios en el repositorio
        await _userRepository.UpdateAsync(userToUpdate, cancellationToken);

        // Mapear la entidad actualizada a un DTO
        response.UpdatedUser = _mapper.Map<UserDto>(userToUpdate);
        response.Success = true;

        return response;
    }
}

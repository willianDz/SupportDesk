using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Contracts.Infraestructure.FileStorage;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Models.Dtos;

namespace SupportDesk.Application.Features.Users.Profile.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, UpdateProfileCommandResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateProfileCommandHandler> _logger;

    public UpdateProfileCommandHandler(
        IUserRepository userRepository,
        IFileStorageService fileStorageService,
        IMapper mapper,
        ILogger<UpdateProfileCommandHandler> logger)
    {
        _userRepository = userRepository;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UpdateProfileCommandResponse> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var response = new UpdateProfileCommandResponse();

        // Validar los parámetros del comando
        await ValidateRequestParameters(request, response, cancellationToken);
        if (!response.Success || response.ValidationErrors?.Count > 0)
        {
            return response;
        }

        // Obtener el usuario desde el repositorio
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            response.Success = false;
            response.Message = UsersMessages.UserNotFound;
            return response;
        }

        if (!user.IsActive)
        {
            response.Success = false;
            response.Message = UsersMessages.UserIsInactive;
            return response;
        }

        // Actualizar los campos del usuario
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.BirthDate = request.DateOfBirth;
        user.GenderId = request.GenderId;

        // Guardar la nueva foto del usuario (si se proporcionó)
        if (request.Photo != null)
        {
            var uploadedUrls = await _fileStorageService.UploadFilesAsync(new List<IFormFile> { request.Photo }, "users", cancellationToken);
            user.PhotoUrl = uploadedUrls.FirstOrDefault();
        }

        // Guardar los cambios en el repositorio
        await _userRepository.UpdateAsync(user, cancellationToken);

        // Mapear la entidad actualizada a un DTO de usuario
        response.UserUpdated = _mapper.Map<UserDto>(user);
        response.Success = true;

        return response;
    }

    private static async Task ValidateRequestParameters(UpdateProfileCommand request, UpdateProfileCommandResponse response, CancellationToken cancellationToken)
    {
        var validator = new UpdateProfileCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.Errors.Count > 0)
        {
            response.Success = false;
            response.ValidationErrors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        }
    }
}

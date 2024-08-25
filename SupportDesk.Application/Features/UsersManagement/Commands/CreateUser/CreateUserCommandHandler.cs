using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SupportDesk.Application.Contracts.Infraestructure.FileStorage;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Models.Dtos;
using SupportDesk.Domain.Entities;
using Microsoft.AspNetCore.Http;
using SupportDesk.Application.Contracts.Services;

namespace SupportDesk.Application.Features.UsersManagement.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserCommandResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IPasswordService _passwordService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateUserCommandHandler> _logger;

    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IFileStorageService fileStorageService,
        IPasswordService passwordService,
        IMapper mapper,
        ILogger<CreateUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _fileStorageService = fileStorageService;
        _passwordService = passwordService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateUserCommandResponse> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var response = new CreateUserCommandResponse();

        // Validar los parámetros del comando
        await ValidateRequestParameters(request, response, cancellationToken);
        if (!response.Success || response.ValidationErrors?.Count > 0)
        {
            return response;
        }

        // Hash de la contraseña
        var hashedPassword = _passwordService.HashPassword(request.Password);

        // Guardar la foto del usuario
        string? photoUrl = null;
        if (request.Photo != null)
        {
            var uploadedUrls = await _fileStorageService.UploadFilesAsync(new List<IFormFile> { request.Photo }, "users", cancellationToken);
            photoUrl = uploadedUrls.FirstOrDefault();
        }

        // Crear la entidad del usuario
        var newUser = new User
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PasswordHash = hashedPassword,
            IsAdmin = request.IsAdmin,
            IsSupervisor = request.IsSupervisor,
            PhotoUrl = photoUrl,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        // Guardar el nuevo usuario en el repositorio
        newUser = await _userRepository.AddAsync(newUser, cancellationToken);

        // Mapear la entidad creada a un DTO de usuario
        response.UserCreated = _mapper.Map<UserDto>(newUser);
        response.Success = true;

        return response;
    }

    private static async Task ValidateRequestParameters(CreateUserCommand request, CreateUserCommandResponse response, CancellationToken cancellationToken)
    {
        var validator = new CreateUserCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.Errors.Count > 0)
        {
            response.Success = false;
            response.ValidationErrors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        }
    }
}

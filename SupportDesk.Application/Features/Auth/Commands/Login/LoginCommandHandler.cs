using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Contracts.Services;

namespace SupportDesk.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginCommandResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITwoFactorService _twoFactorService;
    private readonly IMapper _mapper;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IPasswordService passwordService,
        ITwoFactorService twoFactorService,
        IMapper mapper,
        ILogger<LoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _twoFactorService = twoFactorService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<LoginCommandResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var response = new LoginCommandResponse();

        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null || !_passwordService.VerifyPassword(user.PasswordHash, request.Password))
        {
            response.Success = false;
            response.Message = AuthMessages.InvalidCredentials;
            return response;
        }

        if (!user.IsActive)
        {
            response.Success = false;
            response.Message = UsersMessages.UserIsInactive;
            return response;
        }

        // Generar y enviar el código de autenticación 2FA al usuario
        await _twoFactorService.GenerateAndSendTwoFactorCodeAsync(user.Id, user.Email, cancellationToken);

        response.RequiresTwoFactor = true;
        response.Success = true;

        return response;
    }
}

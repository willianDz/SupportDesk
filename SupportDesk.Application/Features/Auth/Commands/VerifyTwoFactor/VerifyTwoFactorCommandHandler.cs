using MediatR;
using Microsoft.Extensions.Logging;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Contracts.Infraestructure.Security;
using SupportDesk.Application.Constants;

namespace SupportDesk.Application.Features.Auth.Commands.VerifyTwoFactor;

public class VerifyTwoFactorCommandHandler : IRequestHandler<VerifyTwoFactorCommand, VerifyTwoFactorCommandResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly ILogger<VerifyTwoFactorCommandHandler> _logger;

    public VerifyTwoFactorCommandHandler(
        IUserRepository userRepository,
        ITokenGenerator tokenGenerator,
        ILogger<VerifyTwoFactorCommandHandler> logger)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _logger = logger;
    }

    public async Task<VerifyTwoFactorCommandResponse> Handle(VerifyTwoFactorCommand request, CancellationToken cancellationToken)
    {
        var response = new VerifyTwoFactorCommandResponse();

        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null || !await _userRepository.ValidateTwoFactorCodeAsync(user.Id, request.TwoFactorCode, cancellationToken))
        {
            response.Success = false;
            response.Message = AuthMessages.InvalidTwoFactorCode;
            return response;
        }

        // Generar el token utilizando el TokenGenerator
        var tokenRequest = new TokenGenerationRequest
        {
            UserId = user.Id,
            Email = user.Email,
            IsAdmin = user.IsAdmin,
            IsSupervisor = user.IsSupervisor
        };

        response.JwtToken = _tokenGenerator.GenerateToken(tokenRequest, cancellationToken);
        response.Success = true;

        return response;
    }
}

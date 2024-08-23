using MediatR;
using SupportDesk.Application.Contracts.Infraestructure.Security;

namespace SupportDesk.Application.Features.Tokens.Commands.GenerateToken;

public class GenerateTokenCommandHandler : IRequestHandler<GenerateTokenCommand, string>
{
    private readonly ITokenGenerator _tokenGenerator;

    public GenerateTokenCommandHandler(ITokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
    }

    public Task<string> Handle(
        GenerateTokenCommand request, 
        CancellationToken cancellationToken)
    {
        var tokenRequest = new TokenGenerationRequest
        {
            UserId = request.UserId,
            Email = request.Email,
            IsSupervisor = request.IsSupervisor,
            IsAdmin = request.IsAdmin,
            CustomClaims = request.CustomClaims,
        };

        var token = _tokenGenerator.GenerateToken(tokenRequest, cancellationToken);

        return Task.FromResult(token);
    }
}


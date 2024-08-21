using MediatR;
using SupportDesk.Application.Contracts.Infraestructure.Security;

namespace SupportDesk.Application.Features.Tokens.Commands;

public class GenerateTokenCommandHandler : IRequestHandler<GenerateTokenCommand, string>
{
    private readonly ITokenGenerator _tokenGenerator;

    public GenerateTokenCommandHandler(ITokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
    }

    public Task<string> Handle(GenerateTokenCommand request, CancellationToken cancellationToken)
    {
        var tokenRequest = new TokenGenerationRequest
        {
            UserId = request.UserId,
            Email = request.Email,
            IsAdmin = request.IsAdmin,
            CustomClaims = request.CustomClaims,
        };

        var token = _tokenGenerator.GenerateToken(tokenRequest);

        return Task.FromResult(token);
    }
}


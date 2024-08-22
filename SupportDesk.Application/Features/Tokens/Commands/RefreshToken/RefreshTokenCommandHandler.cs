using MediatR;
using SupportDesk.Application.Contracts.Infraestructure.Security;

namespace SupportDesk.Application.Features.Tokens.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, string?>
{
    private readonly ITokenGenerator _tokenGenerator;

    public RefreshTokenCommandHandler(ITokenGenerator tokenGenerator)
    {
        _tokenGenerator = tokenGenerator;
    }

    public Task<string?> Handle(
        RefreshTokenCommand request, 
        CancellationToken cancellationToken)
    {
        var newToken = _tokenGenerator.RefreshToken(request.Token, cancellationToken);

        return Task.FromResult(newToken);
    }
}


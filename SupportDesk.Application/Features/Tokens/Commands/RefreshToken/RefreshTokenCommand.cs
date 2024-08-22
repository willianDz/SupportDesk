using MediatR;

namespace SupportDesk.Application.Features.Tokens.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<string?>
{
    public string Token { get; set; } = string.Empty!;
}


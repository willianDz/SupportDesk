using MediatR;

namespace SupportDesk.Application.Features.Tokens.Commands.GenerateToken;

public class GenerateTokenCommand : IRequest<string>
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty!;
    public bool IsAdmin { get; set; }
    public Dictionary<string, object> CustomClaims { get; set; } = new();
}

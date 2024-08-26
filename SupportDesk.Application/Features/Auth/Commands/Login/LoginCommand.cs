using MediatR;

namespace SupportDesk.Application.Features.Auth.Commands.Login;

public class LoginCommand : IRequest<LoginCommandResponse>
{
    public string Email { get; set; } = string.Empty!;
    public string Password { get; set; } = string.Empty!;
}

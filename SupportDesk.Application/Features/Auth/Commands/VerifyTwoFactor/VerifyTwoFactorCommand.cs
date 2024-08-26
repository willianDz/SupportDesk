using MediatR;

namespace SupportDesk.Application.Features.Auth.Commands.VerifyTwoFactor;

public class VerifyTwoFactorCommand : IRequest<VerifyTwoFactorCommandResponse>
{
    public string Email { get; set; } = string.Empty!;
    public string TwoFactorCode { get; set; } = string.Empty!;
}

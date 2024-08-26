using SupportDesk.Application.Responses;

namespace SupportDesk.Application.Features.Auth.Commands.VerifyTwoFactor;

public class VerifyTwoFactorCommandResponse : BaseResponse
{
    public VerifyTwoFactorCommandResponse() : base()
    {
    }

    public string JwtToken { get; set; } = string.Empty!;
}

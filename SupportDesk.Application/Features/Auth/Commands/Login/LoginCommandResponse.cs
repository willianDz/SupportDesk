using SupportDesk.Application.Models.Dtos;
using SupportDesk.Application.Responses;

namespace SupportDesk.Application.Features.Auth.Commands.Login;

public class LoginCommandResponse : BaseResponse
{
    public LoginCommandResponse() : base()
    {
    }

    public string TwoFactorCode { get; set; } = string.Empty!;
    public bool RequiresTwoFactor { get; set; } = false;
}

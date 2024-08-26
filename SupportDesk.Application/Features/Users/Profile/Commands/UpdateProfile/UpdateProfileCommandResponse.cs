using SupportDesk.Application.Models.Dtos;
using SupportDesk.Application.Responses;

namespace SupportDesk.Application.Features.Users.Profile.Commands.UpdateProfile;

public class UpdateProfileCommandResponse : BaseResponse
{
    public UpdateProfileCommandResponse() : base()
    {
    }

    public UserDto UserUpdated { get; set; } = default!;
}

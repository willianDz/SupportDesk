using SupportDesk.Application.Models.Dtos;
using SupportDesk.Application.Responses;

namespace SupportDesk.Application.Features.UsersManagement.Commands.UpdateUser;

public class UpdateUserCommandResponse : BaseResponse
{
    public UpdateUserCommandResponse() : base()
    {
    }

    public UserDto UpdatedUser { get; set; } = default!;
}

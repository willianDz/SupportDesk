using SupportDesk.Application.Models.Dtos;
using SupportDesk.Application.Responses;

namespace SupportDesk.Application.Features.UsersManagement.Commands.InactivateUser;

public class InactivateUserCommandResponse : BaseResponse
{
    public InactivateUserCommandResponse() : base() { }

    public UserDto? InactivatedUser { get; set; }
}

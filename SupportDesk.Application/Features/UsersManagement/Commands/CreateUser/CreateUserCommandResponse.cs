using SupportDesk.Application.Models.Dtos;
using SupportDesk.Application.Responses;

namespace SupportDesk.Application.Features.UsersManagement.Commands.CreateUser;

public class CreateUserCommandResponse : BaseResponse
{
    public CreateUserCommandResponse() : base()
    {
    }

    public UserDto UserCreated { get; set; } = default!;
}

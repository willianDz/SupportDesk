using SupportDesk.Application.Models.Dtos;
using SupportDesk.Application.Responses;

namespace SupportDesk.Application.Features.UsersManagement.Queries.GetUserById;

public class GetUserByIdQueryResponse : BaseResponse
{
    public UserDto User { get; set; } = null!;
}

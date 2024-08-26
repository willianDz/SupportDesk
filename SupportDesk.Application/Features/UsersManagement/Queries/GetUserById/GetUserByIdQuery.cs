using MediatR;

namespace SupportDesk.Application.Features.UsersManagement.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<GetUserByIdQueryResponse>
{
    public Guid UserId { get; set; }
}

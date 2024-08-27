using MediatR;

namespace SupportDesk.Application.Features.Users.Summary;

public class GetUserSummaryQuery : IRequest<GetUserSummaryQueryResponse>
{
    public Guid UserId { get; set; }
}

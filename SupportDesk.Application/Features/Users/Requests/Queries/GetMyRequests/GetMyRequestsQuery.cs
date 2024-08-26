using MediatR;
using SupportDesk.Application.Models;

namespace SupportDesk.Application.Features.Users.Requests.Queries.GetMyRequests;

public class GetMyRequestsQuery : PagedQuery, IRequest<GetMyRequestsQueryResponse>
{
    public Guid? UserId { get; set; }
    public int? RequestTypeId { get; init; }
    public int? StatusId { get; init; }
    public DateTime? CreatedFrom { get; init; }
    public DateTime? CreatedTo { get; init; }
}

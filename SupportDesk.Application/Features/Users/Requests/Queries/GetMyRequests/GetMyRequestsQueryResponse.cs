using SupportDesk.Application.Models.Dtos;

namespace SupportDesk.Application.Features.Users.Requests.Queries.GetMyRequests;

public class GetMyRequestsQueryResponse
{
    public IReadOnlyList<RequestDto> Requests { get; set; } = new List<RequestDto>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
}

using SupportDesk.Application.Responses;

namespace SupportDesk.Application.Features.Users.Summary;

public class GetUserSummaryQueryResponse : BaseResponse
{
    public int TotalRequests { get; set; }
    public int PendingRequests { get; set; }
    public int RejectedRequests { get; set; }
    public int ApprovedRequests { get; set; }
}

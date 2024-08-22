using SupportDesk.Application.Models.Dtos;
using SupportDesk.Application.Responses;

namespace SupportDesk.Application.Features.Requests.Queries.GetRequestById;

public class GetRequestByIdQueryResponse : BaseResponse
{
    public RequestDto Request { get; set; } = null!;
}

using MediatR;

namespace SupportDesk.Application.Features.Requests.Queries.GetRequestById;

public class GetRequestByIdQuery : IRequest<GetRequestByIdQueryResponse>
{
    public int Id { get; set; }
}
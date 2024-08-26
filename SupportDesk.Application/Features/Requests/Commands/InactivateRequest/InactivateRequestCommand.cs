using MediatR;

namespace SupportDesk.Application.Features.Requests.Commands.InactivateRequest;

public class InactivateRequestCommand : IRequest<InactivateRequestCommandResponse>
{
    public int RequestId { get; set; }
    public Guid UserId { get; set; }
}

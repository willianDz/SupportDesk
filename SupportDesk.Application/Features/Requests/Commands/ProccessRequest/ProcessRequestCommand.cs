using MediatR;

namespace SupportDesk.Application.Features.Requests.Commands.ProcessRequest;

public class ProcessRequestCommand : IRequest<ProcessRequestCommandResponse>
{
    public int RequestId { get; set; }
    public Guid UserId { get; set; }
    public int NewStatusId { get; set; }
    public string? ReviewerUserComments { get; set; }
}

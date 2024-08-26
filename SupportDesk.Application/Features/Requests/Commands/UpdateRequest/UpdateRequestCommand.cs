using MediatR;
using Microsoft.AspNetCore.Http;

namespace SupportDesk.Application.Features.Requests.Commands.UpdateRequest;

public class UpdateRequestCommand : IRequest<UpdateRequestCommandResponse>
{
    public int RequestId { get; set; }
    public Guid UserId { get; set; }
    public int RequestTypeId { get; set; }
    public int ZoneId { get; set; }
    public string Comments { get; set; } = string.Empty!;
    public List<IFormFile>? NewDocuments { get; set; }
    public List<int>? DocumentsToDeactivate { get; set; }
}

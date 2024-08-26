using SupportDesk.Application.Models.Dtos;
using SupportDesk.Application.Responses;

namespace SupportDesk.Application.Features.Requests.Commands.UpdateRequest;

public class UpdateRequestCommandResponse : BaseResponse
{
    public UpdateRequestCommandResponse() : base()
    {
    }

    public RequestDto RequestUpdated { get; set; } = default!;
}

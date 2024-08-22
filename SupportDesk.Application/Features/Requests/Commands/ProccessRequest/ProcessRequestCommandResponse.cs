using SupportDesk.Application.Models.Dtos;
using SupportDesk.Application.Responses;

namespace SupportDesk.Application.Features.Requests.Commands.ProcessRequest
{
    public class ProcessRequestCommandResponse : BaseResponse
    {
        public ProcessRequestCommandResponse() : base()
        {
        }

        public RequestDto ProcessedRequest { get; set; } = default!;
    }
}

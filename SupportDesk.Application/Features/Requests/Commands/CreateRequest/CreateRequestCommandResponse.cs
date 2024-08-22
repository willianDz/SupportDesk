using SupportDesk.Application.Models.Dtos;
using SupportDesk.Application.Responses;

namespace SupportDesk.Application.Features.Requests.Commands.CreateRequest
{
    public class CreateRequestCommandResponse : BaseResponse
    {
        public CreateRequestCommandResponse() : base()
        {
        }

        public RequestDto RequestCreated { get; set; } = default!;
    }

}

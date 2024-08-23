using MediatR;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Contracts.Persistence;

namespace SupportDesk.Application.Features.Requests.Commands.InactivateRequest;

public class InactivateRequestCommandHandler : IRequestHandler<InactivateRequestCommand, InactivateRequestCommandResponse>
{
    private readonly IRequestRepository _requestRepository;

    public InactivateRequestCommandHandler(IRequestRepository requestRepository)
    {
        _requestRepository = requestRepository;
    }

    public async Task<InactivateRequestCommandResponse> Handle(
        InactivateRequestCommand request, 
        CancellationToken cancellationToken)
    {
        var response = new InactivateRequestCommandResponse();

        var requestToInactivate = await _requestRepository.GetByIdAsync(request.RequestId, cancellationToken);

        if (requestToInactivate == null || !requestToInactivate.IsActive)
        {
            response.Success = false;
            response.Message = RequestMessages.RequestNotFoundOrIsInactive;
            return response;
        }

        if (requestToInactivate.CreatedBy != request.UserId)
        {
            response.Success = false;
            response.Message = RequestMessages.InactivateRequestForbidden;
            return response;
        }

        requestToInactivate.IsActive = false;
        requestToInactivate.LastModifiedBy = request.UserId;
        requestToInactivate.LastModifiedDate = DateTime.UtcNow;

        await _requestRepository.UpdateAsync(requestToInactivate, cancellationToken);

        response.Message = RequestMessages.RequestHasBeenInactive;

        return response;
    }
}

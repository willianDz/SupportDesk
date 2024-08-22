using MediatR;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Responses;
using SupportDesk.Domain.Entities;

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
            response.Message = "La solicitud no existe o está inactiva.";
            return response;
        }

        requestToInactivate.IsActive = false;
        requestToInactivate.LastModifiedBy = request.UserId;
        requestToInactivate.LastModifiedDate = DateTime.UtcNow;

        await _requestRepository.UpdateAsync(requestToInactivate, cancellationToken);

        response.Message = "La solicitud ha sido inactivada exitosamente.";

        return response;
    }
}

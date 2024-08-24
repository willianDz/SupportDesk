using MediatR;
using Microsoft.Extensions.Logging;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Models.Notifications;
using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.Features.Requests.Commands.InactivateRequest;

public class InactivateRequestCommandHandler : IRequestHandler<InactivateRequestCommand, InactivateRequestCommandResponse>
{
    private readonly IRequestRepository _requestRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<InactivateRequestCommandHandler> _logger;

    public InactivateRequestCommandHandler(
        IRequestRepository requestRepository,
        INotificationService notificationService,
        ILogger<InactivateRequestCommandHandler> logger)
    {
        _requestRepository = requestRepository;
        _notificationService = notificationService;
        _logger = logger;
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
        
        await NotifySupervisorAsync(requestToInactivate, cancellationToken);

        return response;
    }

    private async Task NotifySupervisorAsync(Request requestToInactivate, CancellationToken cancellationToken)
    {
        if (requestToInactivate.RequestStatusId == (int)RequestStatusesEnum.UnderReview && requestToInactivate.ReviewerUserId.HasValue)
        {
            try
            {
                var notificationMessage = new NotificationMessage
                {
                    RecipientUserIds = new List<Guid> { requestToInactivate.ReviewerUserId.Value },
                    Subject = "Una solicitud ha sido inactivada",
                    Body = $"La solicitud con ID: {requestToInactivate.Id} ha sido inactivada por el solicitante."
                };

                await _notificationService.SendNotificationAsync(notificationMessage, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, NotificationsMessages.FailedToSendNotificationRequestInactivated);
            }
        }
    }
}

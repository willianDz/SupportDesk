using AutoMapper;
using MediatR;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Contracts.Services;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Models.Dtos;
using SupportDesk.Domain.Enums;
using Microsoft.Extensions.Logging;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;
using SupportDesk.Application.Models.Notifications;
using SupportDesk.Domain.Entities;

namespace SupportDesk.Application.Features.Requests.Commands.ProcessRequest;

public class ProcessRequestCommandHandler : IRequestHandler<ProcessRequestCommand, ProcessRequestCommandResponse>
{
    private readonly IRequestRepository _requestRepository;
    private readonly IRequestValidationService _validationService;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    private readonly ILogger<ProcessRequestCommandHandler> _logger;

    public ProcessRequestCommandHandler(
        IRequestRepository requestRepository,
        IRequestValidationService validationService,
        INotificationService notificationService,
        IMapper mapper,
        ILogger<ProcessRequestCommandHandler> logger)
    {
        _requestRepository = requestRepository;
        _validationService = validationService;
        _notificationService = notificationService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProcessRequestCommandResponse> Handle(
        ProcessRequestCommand request,
        CancellationToken cancellationToken)
    {
        var response = new ProcessRequestCommandResponse();

        // Obtener la solicitud desde el repositorio.
        var requestToProcess = await _requestRepository.GetByIdAsync(request.RequestId, cancellationToken);

        if (requestToProcess == null)
        {
            response.Success = false;
            response.Message = RequestMessages.RequestNotFound;
            return response;
        }

        // Validar las reglas de negocio usando el servicio de validación.
        try
        {
            await _validationService.ValidateUserCanProcessRequestAsync(
                requestToProcess,
                request.UserId,
                request.NewStatusId,
                request.ReviewerUserComments,
                cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
            return response;
        }

        // Actualizar el estado de la solicitud.
        if (request.NewStatusId == (int)RequestStatusesEnum.UnderReview)
        {
            requestToProcess.ReviewerUserId = request.UserId;
            requestToProcess.StartReviewDate = DateTime.UtcNow;
        }

        if (request.NewStatusId == (int)RequestStatusesEnum.Approved
            || request.NewStatusId == (int)RequestStatusesEnum.Rejected)
        {
            requestToProcess.ApprovalRejectionDate = DateTime.UtcNow;
        }

        requestToProcess.RequestStatusId = request.NewStatusId;
        requestToProcess.ReviewerUserComments = request.ReviewerUserComments;

        await _requestRepository.UpdateAsync(requestToProcess, cancellationToken);

        // Enviar notificación al solicitante cuando la solicitud comience a ser revisada.
        await NotifyRequesterUnderReviewAsync(requestToProcess, cancellationToken);

        // Enviar notificación al solicitante si la solicitud es aprobada o rechazada.
        await NotifyRequesterAsync(requestToProcess, cancellationToken);

        // Preparar la respuesta.
        response.ProcessedRequest = _mapper.Map<RequestDto>(requestToProcess);

        return response;
    }

    private async Task NotifyRequesterUnderReviewAsync(Request requestToProcess, CancellationToken cancellationToken)
    {
        try
        {
            if (requestToProcess.RequestStatusId == (int)RequestStatusesEnum.UnderReview)
            {
                var notificationMessage = new NotificationMessage
                {
                    RecipientUserIds = new List<Guid> { requestToProcess.CreatedBy!.Value },
                    Subject = $"Solicitud {requestToProcess.Id} en Revisión",
                    Body = $"Su solicitud con ID {requestToProcess.Id} ha comenzado a ser revisada."
                };

                await _notificationService.SendNotificationAsync(notificationMessage, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, NotificationsMessages.FailedToSendNotificationRequestUnderReview);
        }
    }

    private async Task NotifyRequesterAsync(Request requestToProcess, CancellationToken cancellationToken)
    {
        try
        {
            if (requestToProcess.RequestStatusId == (int)RequestStatusesEnum.Approved ||
                requestToProcess.RequestStatusId == (int)RequestStatusesEnum.Rejected)
            {

                string message = $"Su solicitud con ID {requestToProcess.Id} ha sido " +
                           $"{(requestToProcess.RequestStatusId == (int)RequestStatusesEnum.Approved ? "aprobada" : "rechazada")}." +
                           $"Comentarios del supervisor: {requestToProcess.ReviewerUserComments}";

                var notificationMessage = new NotificationMessage
                {
                    RecipientUserIds = new List<Guid> { requestToProcess.CreatedBy!.Value },

                    Subject = requestToProcess.RequestStatusId == (int)RequestStatusesEnum.Approved
                        ? $"Solicitud {requestToProcess.Id} Aprobada"
                        : $"Solicitud {requestToProcess.Id} Rechazada",

                    Body = message
                };

                await _notificationService.SendNotificationAsync(notificationMessage, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, NotificationsMessages.FailedToSendNotificationRequestProcessed);
        }
    }
}

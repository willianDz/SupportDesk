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

        // 1. Obtener la solicitud desde el repositorio.
        var requestToProcess = await _requestRepository.GetByIdAsync(request.RequestId, cancellationToken);

        if (requestToProcess == null)
        {
            response.Success = false;
            response.Message = RequestMessages.RequestNotFound;
            return response;
        }

        // 2. Validar las reglas de negocio usando el servicio de validación.
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

        // 3. Actualizar el estado de la solicitud.
        if (request.NewStatusId == (int)RequestStatusesEnum.UnderReview)
        {
            requestToProcess.ReviewerUserId = request.UserId;
            requestToProcess.StartReviewDate = DateTime.UtcNow;
        }

        requestToProcess.RequestStatusId = request.NewStatusId;
        requestToProcess.ReviewerUserComments = request.ReviewerUserComments;
        requestToProcess.LastModifiedBy = request.UserId;
        requestToProcess.LastModifiedDate = DateTime.UtcNow;

        await _requestRepository.UpdateAsync(requestToProcess, cancellationToken);

        // 4. Enviar notificación al solicitante si la solicitud es aprobada o rechazada.
        await NotifyRequesterAsync(requestToProcess, cancellationToken);

        // 5. Preparar la respuesta.
        response.ProcessedRequest = _mapper.Map<RequestDto>(requestToProcess);

        return response;
    }

    private async Task NotifyRequesterAsync(Request requestToProcess, CancellationToken cancellationToken)
    {
        try
        {
            if (requestToProcess.RequestStatusId == (int)RequestStatusesEnum.Approved ||
                requestToProcess.RequestStatusId == (int)RequestStatusesEnum.Rejected)
            {
                var notificationMessage = new NotificationMessage
                {
                    RecipientUserIds = new List<Guid> { requestToProcess.CreatedBy!.Value },

                    Subject = requestToProcess.RequestStatusId == (int)RequestStatusesEnum.Approved
                        ? "Solicitud Aprobada"
                        : "Solicitud Rechazada",

                    Body = $"Su solicitud con ID {requestToProcess.Id} ha sido " +
                           $"{(requestToProcess.RequestStatusId == (int)RequestStatusesEnum.Approved ? "aprobada" : "rechazada")}."
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

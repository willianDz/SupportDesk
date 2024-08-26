using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Contracts.Infraestructure.FileStorage;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Contracts.Services;
using SupportDesk.Application.Models.Dtos;
using SupportDesk.Application.Models.Notifications;
using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.Features.Requests.Commands.UpdateRequest;

public class UpdateRequestCommandHandler : IRequestHandler<UpdateRequestCommand, UpdateRequestCommandResponse>
{
    private readonly IRequestRepository _requestRepository;
    private readonly IRequestValidationService _validationService;
    private readonly IFileStorageService _fileStorageService;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateRequestCommandHandler> _logger;

    public UpdateRequestCommandHandler(
        IRequestRepository requestRepository,
        IRequestValidationService validationService,
        IFileStorageService fileStorageService,
        INotificationService notificationService,
        IMapper mapper,
        ILogger<UpdateRequestCommandHandler> logger)
    {
        _requestRepository = requestRepository;
        _validationService = validationService;
        _fileStorageService = fileStorageService;
        _notificationService = notificationService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UpdateRequestCommandResponse> Handle(
        UpdateRequestCommand request, 
        CancellationToken cancellationToken)
    {
        var response = new UpdateRequestCommandResponse();

        var requestToUpdate = await _requestRepository.GetByIdAsync(request.RequestId, cancellationToken);

        if (requestToUpdate == null || !requestToUpdate.IsActive)
        {
            response.Success = false;
            response.Message = RequestMessages.RequestNotFoundOrIsInactive;
            return response;
        }

        // Validar las reglas de negocio usando el servicio de validación.
        try
        {
            await _validationService.ValidateUserCanUpdateHisRequestAsync(
                requestToUpdate, 
                request.RequestTypeId, 
                request.ZoneId, 
                cancellationToken);
        }
        catch (InvalidOperationException ex)
        {
            response.Success = false;
            response.Message = ex.Message;
            return response;
        }

        // Actualizar los campos de la solicitud
        requestToUpdate.RequestTypeId = request.RequestTypeId;
        requestToUpdate.ZoneId = request.ZoneId;
        requestToUpdate.Comments = request.Comments;
        requestToUpdate.LastModifiedBy = request.UserId;
        requestToUpdate.LastModifiedDate = DateTime.UtcNow;

        // Desactivar documentos si es necesario
        if (request.DocumentsToDeactivate != null)
        {
            foreach (var docId in request.DocumentsToDeactivate)
            {
                var doc = requestToUpdate.RequestDocuments?.FirstOrDefault(d => d.Id == docId);

                if (doc != null)
                {
                    doc.IsActive = false;
                    doc.LastModifiedBy = request.UserId;
                    doc.LastModifiedDate = DateTime.UtcNow;
                }
            }
        }

        // Agregar nuevos documentos si los hay
        if (request.NewDocuments != null && request.NewDocuments.Count > 0)
        {
            var newDocumentUrls = await _fileStorageService
                .UploadFilesAsync(request.NewDocuments, "requests", cancellationToken);
            
            foreach (var url in newDocumentUrls)
            {
                requestToUpdate.RequestDocuments?.Add(new RequestDocument
                {
                    DocumentUrl = url,
                    IsActive = true,
                    CreatedBy = request.UserId,
                    CreatedDate = DateTime.UtcNow,
                });
            }
        }

        await _requestRepository.UpdateAsync(requestToUpdate, cancellationToken);

        response.RequestUpdated = _mapper.Map<RequestDto>(requestToUpdate);
        response.Message = RequestMessages.RequestHasBeenUpdated;

        if (requestToUpdate.RequestStatusId == (int)RequestStatusesEnum.UnderReview)
        {
            await NotifySupervisorAsync(requestToUpdate, cancellationToken);
        }

        return response;
    }

    private async Task NotifySupervisorAsync(Request updatedRequest, CancellationToken cancellationToken)
    {
        try
        {
            if (updatedRequest.ReviewerUserId == null)
            {
                _logger.LogWarning(NotificationsMessages.NoSupervisorAssignedToUpdatedRequest);
                return;
            }

            var notificationMessage = new NotificationMessage
            {
                RecipientUserIds = new List<Guid> { updatedRequest.ReviewerUserId.Value },
                Subject = "Solicitud actualizada por el solicitante",
                Body = $"La solicitud con ID {updatedRequest.Id} ha sido actualizada por el solicitante."
            };

            await _notificationService.SendNotificationAsync(notificationMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, NotificationsMessages.FailedToSendNotificationRequestUpdate);
        }
    }
}

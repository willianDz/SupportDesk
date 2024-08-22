using AutoMapper;
using MediatR;
using SupportDesk.Application.Contracts.Infraestructure.FileStorage;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Models.Dtos;
using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.Features.Requests.Commands.UpdateRequest;

public class UpdateRequestCommandHandler : IRequestHandler<UpdateRequestCommand, UpdateRequestCommandResponse>
{
    private readonly IRequestRepository _requestRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;

    public UpdateRequestCommandHandler(
        IRequestRepository requestRepository,
        IFileStorageService fileStorageService,
        IMapper mapper)
    {
        _requestRepository = requestRepository;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
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
            response.Message = "La solicitud no existe o está inactiva.";
            return response;
        }

        // Validar que no se pueda actualizar si está en estado Aprobado o Rechazado
        if (requestToUpdate.RequestStatusId == (int)RequestStatusesEnum.Approved ||
            requestToUpdate.RequestStatusId == (int)RequestStatusesEnum.Rejected)
        {
            response.Success = false;
            response.Message = "No se puede actualizar una solicitud que ya ha sido procesada.";
            return response;
        }

        // Validar que no se pueda actualizar el tipo de solicitud o zona si está en estado En Revision
        if (requestToUpdate.RequestStatusId == (int)RequestStatusesEnum.UnderReview &&
            (requestToUpdate.RequestTypeId != request.RequestTypeId || requestToUpdate.ZoneId != request.ZoneId))
        {
            response.Success = false;
            response.Message = "No se puede actualizar el tipo de solicitud o zona cuando la solicitud ya se encuentra en revisión.";
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
        response.Message = "Solicitud actualizada exitosamente.";
        return response;
    }
}

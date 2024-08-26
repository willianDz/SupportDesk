using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Contracts.Infraestructure.FileStorage;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Models.Dtos;
using SupportDesk.Application.Models.Notifications;
using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.Features.Requests.Commands.CreateRequest;

public class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, CreateRequestCommandResponse>
{
    private readonly IRequestRepository _requestRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly INotificationService _notificationService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateRequestCommandHandler> _logger;

    public CreateRequestCommandHandler(
        IRequestRepository requestRepository,
        IUserRepository userRepository,
        IFileStorageService fileStorageService,
        INotificationService notificationService,
        IMapper mapper,
        ILogger<CreateRequestCommandHandler> logger)
    {
        _requestRepository = requestRepository;
        _userRepository = userRepository;
        _fileStorageService = fileStorageService;
        _notificationService = notificationService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateRequestCommandResponse> Handle(
        CreateRequestCommand request, 
        CancellationToken cancellationToken)
    {
        var response = new CreateRequestCommandResponse();

        await ValidateRequestParameters(request, response, cancellationToken);

        if (!response.Success || response.ValidationErrors?.Count > 0)
        {
            return response;
        }

        var documentUrls = new List<string>();

        if (request.Documents != null && request.Documents.Any())
        {
            documentUrls = await _fileStorageService
                .UploadFilesAsync(request.Documents, "requests", cancellationToken);
        }

        var newRequest = new Request
        {
            RequestTypeId = request.RequestTypeId,
            ZoneId = request.ZoneId,
            Comments = request.Comments,
            RequestStatusId = (int)RequestStatusesEnum.New,
            CreatedBy = request.UserId,
            CreatedDate = DateTime.UtcNow,
            RequestDocuments = documentUrls.Select(url => new RequestDocument
            {
                DocumentUrl = url,
                CreatedBy = request.UserId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
            }).ToList()
        };

        newRequest = await _requestRepository.AddAsync(newRequest, cancellationToken);

        response.RequestCreated = _mapper.Map<RequestDto>(newRequest);

        await NotifySupervisorsAndAdminsAsync(newRequest, cancellationToken);

        return response;
    }

    private static async Task ValidateRequestParameters(
        CreateRequestCommand request, 
        CreateRequestCommandResponse response,
        CancellationToken cancellationToken)
    {
        var validator = new CreateRequestCommandValidator();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (validationResult.Errors.Count > 0)
        {
            response.Success = false;

            response.ValidationErrors = validationResult
                .Errors
                .Select(e => e.ErrorMessage)
                .ToList();
        }
    }

    private async Task NotifySupervisorsAndAdminsAsync(
        Request newRequest, 
        CancellationToken cancellationToken)
    {
        try
        {
            var recipientUserIds = await _userRepository.GetSupervisorsAndAdminsForRequestAsync(
                newRequest.RequestTypeId,
                newRequest.ZoneId,
                cancellationToken);

            if (recipientUserIds.Count == 0)
            {
                return;
            }

            var notificationMessage = new NotificationMessage
            {
                RecipientUserIds = recipientUserIds.Select(u => u.Id).ToList(),
                Subject = "Nueva solicitud registrada",
                Body = $"Una nueva solicitud ha sido registrada en el sistema. ID de solicitud: {newRequest.Id}."
            };

            await _notificationService.SendNotificationAsync(notificationMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, NotificationsMessages.FailedToSendNotificationRequestCreated);
        }
    }
}

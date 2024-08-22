using AutoMapper;
using MediatR;
using SupportDesk.Application.Contracts.Infraestructure.FileStorage;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Models.Dtos;
using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.Features.Requests.Commands.CreateRequest;

public class CreateRequestCommandHandler : IRequestHandler<CreateRequestCommand, CreateRequestCommandResponse>
{
    private readonly IRequestRepository _requestRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;

    public CreateRequestCommandHandler(
        IRequestRepository requestRepository,
        IFileStorageService fileStorageService,
        IMapper mapper)
    {
        _requestRepository = requestRepository;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
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
}

using AutoMapper;
using MediatR;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Contracts.Services;
using SupportDesk.Application.Models.Dtos;

namespace SupportDesk.Application.Features.Requests.Queries.GetRequestById;

public class GetRequestByIdQueryHandler : IRequestHandler<GetRequestByIdQuery, GetRequestByIdQueryResponse>
{
    private readonly IRequestRepository _requestRepository;
    private readonly IMapper _mapper;

    public GetRequestByIdQueryHandler(
        IRequestRepository requestRepository,
        IMapper mapper)
    {
        _requestRepository = requestRepository;
        _mapper = mapper;
    }

    public async Task<GetRequestByIdQueryResponse> Handle(
        GetRequestByIdQuery query,
        CancellationToken cancellationToken)
    {
        var response = new GetRequestByIdQueryResponse();

        var request = await _requestRepository.GetByIdAsync(query.Id, cancellationToken);

        // Validaciones
        if (request == null)
        {
            response.Success = false;
            response.Message = RequestMessages.RequestNotFound;
            return response;
        }

        if (!request.IsActive)
        {
            response.Success = false;
            response.Message = RequestMessages.RequestIsInactive;
            return response;
        }

        response.Request = _mapper.Map<RequestDto>(request);
        return response;
    }
}

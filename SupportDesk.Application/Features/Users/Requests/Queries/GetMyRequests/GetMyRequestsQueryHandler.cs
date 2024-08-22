using AutoMapper;
using MediatR;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Features.Users.Requests.Queries.GetMyRequests;
using SupportDesk.Application.Models;
using SupportDesk.Application.Models.Dtos;

namespace SupportDesk.Application.Features.Requests.Queries.GetMyRequests;

public class GetMyRequestsQueryHandler : IRequestHandler<GetMyRequestsQuery, GetMyRequestsQueryResponse>
{
    private readonly IRequestRepository _requestRepository;
    private readonly IMapper _mapper;

    public GetMyRequestsQueryHandler(
        IRequestRepository requestRepository,
        IMapper mapper)
    {
        _requestRepository = requestRepository;
        _mapper = mapper;
    }

    public async Task<GetMyRequestsQueryResponse> Handle(
        GetMyRequestsQuery query,
        CancellationToken cancellationToken)
    {
        var (requests, totalCount) = await _requestRepository.GetUserRequestsAsync(
            userId: query.UserId!.Value,
            requestTypeId: query.RequestTypeId,
            statusId: query.StatusId,
            createdFrom: query.CreatedFrom,
            createdTo: query.CreatedTo,
            page: query.Page ?? PagedQuery.DefaultPage,
            pageSize: query.PageSize ?? PagedQuery.DefaultPageSize,
            cancellationToken: cancellationToken);

        var requestDtos = _mapper.Map<IReadOnlyList<RequestDto>>(requests);

        return new GetMyRequestsQueryResponse
        {
            Requests = requestDtos,
            Page = query.Page ?? PagedQuery.DefaultPage,
            PageSize = query.PageSize ?? PagedQuery.DefaultPageSize,
            TotalCount = totalCount
        };
    }
}

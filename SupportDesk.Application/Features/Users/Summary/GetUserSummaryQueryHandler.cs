using MediatR;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Constants;
using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.Features.Users.Summary;

public class GetUserSummaryQueryHandler : IRequestHandler<GetUserSummaryQuery, GetUserSummaryQueryResponse>
{
    private readonly IRequestRepository _requestRepository;
    private readonly IUserRepository _userRepository;

    public GetUserSummaryQueryHandler(IRequestRepository requestRepository, IUserRepository userRepository)
    {
        _requestRepository = requestRepository;
        _userRepository = userRepository;
    }

    public async Task<GetUserSummaryQueryResponse> Handle(GetUserSummaryQuery query, CancellationToken cancellationToken)
    {
        var response = new GetUserSummaryQueryResponse();

        var user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);

        if (user == null)
        {
            response.Success = false;
            response.Message = UsersMessages.UserNotFound;
            return response;
        }

        if (!user.IsActive)
        {
            response.Success = false;
            response.Message = UsersMessages.UserIsInactive;
            return response;
        }

        response.TotalRequests = await _requestRepository.CountAsync(r => r.CreatedBy == query.UserId, cancellationToken);
        response.PendingRequests = await _requestRepository.CountAsync(r => r.CreatedBy == query.UserId && r.RequestStatusId == (int)RequestStatusesEnum.New, cancellationToken);
        response.RejectedRequests = await _requestRepository.CountAsync(r => r.CreatedBy == query.UserId && r.RequestStatusId == (int)RequestStatusesEnum.Rejected, cancellationToken);
        response.ApprovedRequests = await _requestRepository.CountAsync(r => r.CreatedBy == query.UserId && r.RequestStatusId == (int)RequestStatusesEnum.Approved, cancellationToken);

        response.Success = true;
        return response;
    }
}

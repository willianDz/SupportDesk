using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Contracts.Services;
using SupportDesk.Application.Constants;
using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.Services;

public class RequestValidationService : IRequestValidationService
{
    private readonly IUserRepository _userRepository;

    public RequestValidationService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> ValidateUserCanProcessRequestAsync(
        Request request, 
        Guid userId, 
        int newStatusId, 
        CancellationToken cancellationToken = default)
    {
        if (!request.IsActive)
        {
            throw new InvalidOperationException(RequestMessages.InactiveRequestCannotBeProcessed);
        }

        if (request.RequestStatusId == (int)RequestStatusesEnum.Approved ||
            request.RequestStatusId == (int)RequestStatusesEnum.Rejected)
        {
            throw new InvalidOperationException(RequestMessages.RequestAlreadyApprovedOrRejected);
        }

        if (request.ReviewerUserId.HasValue && request.ReviewerUserId != userId)
        {
            throw new InvalidOperationException(RequestMessages.RequestAlreadyBeingReviewed);
        }

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException(RequestMessages.UserNotFound);
        }

        var hasZonePermission = user.UserZones?.Any(uz => uz.ZoneId == request.ZoneId);
        
        if (hasZonePermission != null && !hasZonePermission.Value)
        {
            throw new InvalidOperationException(RequestMessages.UserNoZonePermission);
        }

        var hasRequestTypePermission = user.UserRequestTypes?.Any(urt => urt.RequestTypeId == request.RequestTypeId);
        
        if (hasRequestTypePermission != null && !hasRequestTypePermission.Value)
        {
            throw new InvalidOperationException(RequestMessages.UserNoRequestTypePermission);
        }

        return true;
    }
}

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
        string? reviewerUserComments,
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

        if (string.IsNullOrEmpty(reviewerUserComments) 
            && (newStatusId == (int)RequestStatusesEnum.Approved || newStatusId == (int)RequestStatusesEnum.Rejected))
        {
            throw new InvalidOperationException(RequestMessages.ReviewerUserCommentsAreRequired);
        }

        if ((newStatusId == (int)RequestStatusesEnum.Approved || newStatusId == (int)RequestStatusesEnum.Rejected) 
            && reviewerUserComments!.Length < 15)
        {
            throw new InvalidOperationException(RequestMessages.CommentsMinLenght);
        }

        if ((newStatusId == (int)RequestStatusesEnum.Approved || newStatusId == (int)RequestStatusesEnum.Rejected)
            && reviewerUserComments!.Length > 800)
        {
            throw new InvalidOperationException(RequestMessages.CommentsMaxLenght);
        }

        return true;
    }

    public Task<bool> ValidateUserCanUpdateHisRequestAsync(
        Request request,
        int newRequestTypeId,
        int newZoneId,
        CancellationToken cancellationToken = default)
    {
        // Validar que no se pueda actualizar si está en estado Aprobado o Rechazado
        if (request.RequestStatusId == (int)RequestStatusesEnum.Approved ||
            request.RequestStatusId == (int)RequestStatusesEnum.Rejected)
        {
            throw new InvalidOperationException(RequestMessages.RequestAlreadyProccessed);
        }

        // Validar que no se pueda actualizar el tipo de solicitud o zona si está en estado En Revision
        if (request.RequestStatusId == (int)RequestStatusesEnum.UnderReview &&
            (request.RequestTypeId != newRequestTypeId || request.ZoneId != newZoneId))
        {
            throw new InvalidOperationException(RequestMessages.CannotUpdateRequestTypeOrZone);
        }

        return Task.FromResult(true);
    }
}

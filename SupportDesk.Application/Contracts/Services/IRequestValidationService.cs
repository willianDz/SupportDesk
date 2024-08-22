using SupportDesk.Domain.Entities;

namespace SupportDesk.Application.Contracts.Services
{
    public interface IRequestValidationService
    {
        Task<bool> ValidateUserCanProcessRequestAsync(
            Request request,
            Guid userId,
            int newStatusId,
            CancellationToken cancellationToken = default);

        Task<bool> ValidateUserCanUpdateHisRequestAsync(
            Request request,
            int newRequestTypeId,
            int newZoneId,
            CancellationToken cancellationToken = default);
    }
}

using SupportDesk.Domain.Entities;

namespace SupportDesk.Application.Contracts.Persistence;

public interface IRequestRepository : IAsyncRepository<Request>
{
    Task<(IReadOnlyList<Request> Requests, int TotalCount)> GetUserRequestsAsync(
        Guid userId,
        int? requestTypeId,
        int? statusId,
        DateTime? createdFrom,
        DateTime? createdTo,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Request>> GetPendingRequestsAsync(
            TimeSpan pendingThreshold,
            CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Request>> GetExpiringRequestsAsync(
        TimeSpan expiringThreshold, 
        CancellationToken cancellationToken = default);
}

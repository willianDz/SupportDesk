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

    Task<int> GetRequestsCountByDateAsync(
        DateTime date, 
        CancellationToken cancellationToken);
    
    Task<int> GetProcessedRequestsCountByDateAsync(
        DateTime date, 
        CancellationToken cancellationToken);
    
    Task<int> GetPendingRequestsCountAsync(CancellationToken cancellationToken);
    
    Task<TimeSpan> GetAverageResponseTimeAsync(CancellationToken cancellationToken);

    Task<int> GetRequestCountByStatusAsync(
        int statusId, 
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);
    
    Task<Dictionary<int, int>> GetWeeklyRequestCountsAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);
    
    Task<List<(int RequestTypeId, int ZoneId, int Count)>> GetRequestTrendsByTypeAndZoneAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);
    
    Task<TimeSpan> GetAverageResolutionTimeAsync(
        DateTime startDate, 
        DateTime endDate, 
        CancellationToken cancellationToken = default);
}

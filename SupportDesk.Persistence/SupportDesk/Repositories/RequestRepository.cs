using Microsoft.EntityFrameworkCore;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;
using System.Linq.Expressions;

namespace SupportDesk.Persistence.SupportDesk.Repositories
{
    public class RequestRepository : BaseRepository<Request>, IRequestRepository
    {
        public RequestRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        override
        public async Task<Request?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Requests
                .Include(r => r.RequestType)
                .Include(r => r.Zone)
                .Include(r => r.ReviewerUser)
                .Include(r => r.RequestDocuments)
                .SingleOrDefaultAsync(r => r.Id == id, cancellationToken);
        }

        public async Task<(IReadOnlyList<Request> Requests, int TotalCount)> GetUserRequestsAsync(
        Guid userId,
        int? requestTypeId,
        int? statusId,
        DateTime? createdFrom,
        DateTime? createdTo,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
        {
            var queryable = _dbContext.Requests
                .Where(r => r.CreatedBy == userId);

            // Aplicar filtros
            if (requestTypeId.HasValue)
            {
                queryable = queryable.Where(r => r.RequestTypeId == requestTypeId.Value);
            }

            if (statusId.HasValue)
            {
                queryable = queryable.Where(r => r.RequestStatusId == statusId.Value);
            }

            if (createdFrom.HasValue)
            {
                queryable = queryable.Where(r => r.CreatedDate.Date >= createdFrom.Value.Date);
            }

            if (createdTo.HasValue)
            {
                queryable = queryable.Where(r => r.CreatedDate.Date <= createdTo.Value.Date);
            }

            // Obtener el total antes de aplicar paginación
            var totalCount = await queryable.CountAsync(cancellationToken);

            // Aplicar paginación
            var requests = await queryable
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (requests, totalCount);
        }

        public async Task<IReadOnlyList<Request>> GetPendingRequestsAsync(
            TimeSpan pendingThreshold,
            CancellationToken cancellationToken = default)
        {
            var thresholdTime = DateTime.UtcNow.Subtract(pendingThreshold);

            return await _dbContext.Requests
                .Where(r => r.ReviewerUserId == null
                            && r.CreatedDate <= thresholdTime
                            && r.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Request>> GetExpiringRequestsAsync(
            TimeSpan expiringThreshold,
            CancellationToken cancellationToken = default)
        {
            var thresholdTime = DateTime.UtcNow.Subtract(expiringThreshold);

            return await _dbContext.Requests
                .Where(r => r.CreatedDate <= thresholdTime &&
                            r.RequestStatusId != (int)RequestStatusesEnum.Approved &&
                            r.RequestStatusId != (int)RequestStatusesEnum.Rejected &&
                            r.IsActive)
                .Include(r => r.ReviewerUser)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetRequestsCountByDateAsync(DateTime date, CancellationToken cancellationToken)
        {
            return await _dbContext.Requests
                .CountAsync(r => r.CreatedDate.Date == date.Date && r.IsActive, cancellationToken);
        }

        public async Task<int> GetProcessedRequestsCountByDateAsync(DateTime date, CancellationToken cancellationToken)
        {
            return await _dbContext.Requests
                .CountAsync(r => r.ApprovalRejectionDate != null &&
                                 r.ApprovalRejectionDate.Value.Date == date.Date &&
                                 (r.RequestStatusId == (int)RequestStatusesEnum.Approved ||
                                  r.RequestStatusId == (int)RequestStatusesEnum.Rejected) &&
                                 r.IsActive, cancellationToken);
        }

        public async Task<int> GetPendingRequestsCountAsync(CancellationToken cancellationToken)
        {
            return await _dbContext.Requests
                .CountAsync(r => r.ReviewerUserId == null &&
                                 r.RequestStatusId != (int)RequestStatusesEnum.Approved &&
                                 r.RequestStatusId != (int)RequestStatusesEnum.Rejected &&
                                 r.IsActive, cancellationToken);
        }

        public async Task<TimeSpan> GetAverageResponseTimeAsync(CancellationToken cancellationToken)
        {
            var requestTimes = await _dbContext.Requests
                .Where(r => r.ReviewerUserId != null &&
                            r.RequestStatusId == (int)RequestStatusesEnum.Approved &&
                            r.ApprovalRejectionDate != null)
                .Select(r => new
                {
                    r.CreatedDate,
                    ApprovalRejectionDate = r.ApprovalRejectionDate.Value
                })
                .ToListAsync(cancellationToken);

            if (!requestTimes.Any())
            {
                return TimeSpan.Zero;
            }

            var averageTicks = requestTimes
                .Average(rt => (rt.ApprovalRejectionDate - rt.CreatedDate).Ticks);

            return TimeSpan.FromTicks((long)averageTicks);
        }


        public async Task<int> GetRequestCountByStatusAsync(
            int statusId, 
            DateTime startDate, 
            DateTime endDate, 
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Requests
                .Where(r => r.RequestStatusId == statusId && r.CreatedDate >= startDate && r.CreatedDate <= endDate && r.IsActive)
                .CountAsync(cancellationToken);
        }

        public async Task<Dictionary<int, int>> GetWeeklyRequestCountsAsync(
            DateTime startDate, 
            DateTime endDate, 
            CancellationToken cancellationToken = default)
        {
            var requests = await _dbContext.Requests
                .Where(r => r.CreatedDate >= startDate && r.CreatedDate <= endDate && r.IsActive)
                .ToListAsync(cancellationToken);

            return requests
                .GroupBy(r => r.RequestStatusId)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        public async Task<List<(int RequestTypeId, int ZoneId, int Count)>> GetRequestTrendsByTypeAndZoneAsync(
            DateTime startDate, 
            DateTime endDate, 
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Requests
                .Where(r => r.CreatedDate >= startDate && r.CreatedDate <= endDate && r.IsActive)
                .GroupBy(r => new { r.RequestTypeId, r.ZoneId })
                .Select(g => new { g.Key.RequestTypeId, g.Key.ZoneId, Count = g.Count() })
                .ToListAsync(cancellationToken)
                .ContinueWith(task => task.Result
                    .Select(r => (r.RequestTypeId, r.ZoneId, r.Count))
                    .ToList());
        }

        public async Task<TimeSpan> GetAverageResolutionTimeAsync(
            DateTime startDate, 
            DateTime endDate, 
            CancellationToken cancellationToken = default)
        {
            var averageTicks = await _dbContext.Requests
                .Where(r => r.ReviewerUserId != null &&
                            r.RequestStatusId == (int)RequestStatusesEnum.Approved &&
                            r.CreatedDate >= startDate && r.CreatedDate <= endDate)
                .AverageAsync(r => (r.ApprovalRejectionDate!.Value - r.CreatedDate).Ticks, cancellationToken);

            return TimeSpan.FromTicks(Convert.ToInt64(averageTicks));
        }

        public async Task<int> CountAsync(Expression<Func<Request, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Requests.CountAsync(predicate, cancellationToken);
        }
    }
}

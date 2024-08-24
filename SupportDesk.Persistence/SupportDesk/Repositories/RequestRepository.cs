﻿using Microsoft.EntityFrameworkCore;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;

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
                .CountAsync(r => r.LastModifiedDate != null &&
                                 r.LastModifiedDate.Value.Date == date.Date &&
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
                            r.LastModifiedDate != null)
                .Select(r => new
                {
                    r.CreatedDate,
                    LastModifiedDate = r.LastModifiedDate.Value
                })
                .ToListAsync(cancellationToken);

            if (!requestTimes.Any())
            {
                return TimeSpan.Zero;
            }

            var averageTicks = requestTimes
                .Average(rt => (rt.LastModifiedDate - rt.CreatedDate).Ticks);

            return TimeSpan.FromTicks((long)averageTicks);
        }
    }
}

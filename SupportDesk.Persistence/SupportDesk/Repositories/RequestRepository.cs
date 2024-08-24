using Microsoft.EntityFrameworkCore;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Domain.Entities;

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
    }
}

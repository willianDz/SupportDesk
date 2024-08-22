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
                .Where(r => r.ReviewerUserId == userId);

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
                queryable = queryable.Where(r => r.CreatedDate >= createdFrom.Value);
            }

            if (createdTo.HasValue)
            {
                queryable = queryable.Where(r => r.CreatedDate <= createdTo.Value);
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
    }
}

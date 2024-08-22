using Microsoft.EntityFrameworkCore;
using SupportDesk.Application.Contracts.Persistence;

namespace SupportDesk.Persistence.SupportDesk.Repositories
{
    public class BaseRepository<T> : IAsyncRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _dbContext;

        public BaseRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            T? t = await _dbContext
                .Set<T>()
                .FindAsync(id, cancellationToken);

            return t;
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            T? t = await _dbContext
                .Set<T>()
                .FindAsync(id, cancellationToken);

            return t;
        }

        public async Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Set<T>().ToListAsync(cancellationToken);
        }

        public async virtual Task<IReadOnlyList<T>> GetPagedReponseAsync(
            int page,
            int size,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext
                .Set<T>()
                .Skip((page - 1) * size)
                .Take(size)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.Set<T>().AddAsync(entity, cancellationToken);

            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task<List<T>> AddRangeAsync(List<T> entities, CancellationToken cancellationToken = default)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities, cancellationToken);
            await _dbContext.SaveChangesAsync();

            return entities;
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}

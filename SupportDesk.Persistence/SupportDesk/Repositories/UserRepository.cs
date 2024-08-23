using Microsoft.EntityFrameworkCore;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Domain.Entities;

namespace SupportDesk.Persistence.SupportDesk.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<User>> GetUsersByIdsAsync(
            List<Guid> userIds, 
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.Users
                .Where(user => userIds.Contains(user.Id))
                .ToListAsync(cancellationToken);
        }
    }
}

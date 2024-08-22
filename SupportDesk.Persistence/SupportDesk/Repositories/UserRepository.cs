using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Domain.Entities;

namespace SupportDesk.Persistence.SupportDesk.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}

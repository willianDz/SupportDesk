using SupportDesk.Domain.Entities;

namespace SupportDesk.Application.Contracts.Persistence;

public interface IUserRepository : IAsyncRepository<User>
{
}

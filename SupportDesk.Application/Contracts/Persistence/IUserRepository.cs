using SupportDesk.Domain.Entities;

namespace SupportDesk.Application.Contracts.Persistence;

public interface IUserRepository : IAsyncRepository<User>
{
    Task<List<User>> GetUsersByIdsAsync(
        List<Guid> userIds, 
        CancellationToken cancellationToken = default);

    Task<List<User>> GetSupervisorsAndAdminsForRequestAsync(
        int requestTypeId, 
        int zoneId, 
        CancellationToken cancellationToken = default);
}

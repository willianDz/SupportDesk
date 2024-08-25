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

    Task<List<User>> GetSupervisorsByZoneAndRequestTypeAsync(
        int zoneId, 
        int requestTypeId, 
        CancellationToken cancellationToken = default);

    Task<List<User>> GetAdminUsersAsync(
        CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task SaveTwoFactorCodeAsync(Guid userId, string twoFactorCode, CancellationToken cancellationToken = default);

    Task<bool> ValidateTwoFactorCodeAsync(Guid userId, string twoFactorCode, CancellationToken cancellationToken = default);
}

using Microsoft.EntityFrameworkCore;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Domain.Entities;

namespace SupportDesk.Persistence.SupportDesk.Repositories;

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

    public async Task<List<User>> GetSupervisorsAndAdminsForRequestAsync(
        int requestTypeId,
        int zoneId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(u => u.UserRequestTypes)
            .Include(u => u.UserZones)
            .Where(u => u.IsActive &&
                        (u.IsAdmin || u.IsSupervisor) &&
                        u.UserRequestTypes!.Any(urt => urt.RequestTypeId == requestTypeId) &&
                        u.UserZones!.Any(uz => uz.ZoneId == zoneId))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetSupervisorsByZoneAndRequestTypeAsync(
        int zoneId,
        int requestTypeId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Include(u => u.UserZones)
            .Include(u => u.UserRequestTypes)
            .Where(u => u.IsSupervisor &&
                        u.UserZones!.Any(uz => uz.ZoneId == zoneId) &&
                        u.UserRequestTypes!.Any(urt => urt.RequestTypeId == requestTypeId))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<User>> GetAdminUsersAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Where(u => u.IsAdmin && u.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive, cancellationToken);
    }

    public async Task SaveTwoFactorCodeAsync(Guid userId, string twoFactorCode, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FindAsync(new object[] { userId }, cancellationToken);

        if (user == null)
        {
            throw new InvalidOperationException(UsersMessages.UserNotFound);
        }

        user.TwoFactorCode = twoFactorCode;
        user.TwoFactorCodeExpiration = DateTime.UtcNow.AddMinutes(10); // Código válido por 10 minutos

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ValidateTwoFactorCodeAsync(Guid userId, string twoFactorCode, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FindAsync(new object[] { userId }, cancellationToken);

        if (user == null || user.TwoFactorCodeExpiration < DateTime.UtcNow)
        {
            return false;
        }

        return user.TwoFactorCode == twoFactorCode;
    }
}

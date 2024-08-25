namespace SupportDesk.Application.Contracts.Services;

public interface ITwoFactorService
{
    Task<string> GenerateAndSendTwoFactorCodeAsync(Guid userId, string email, CancellationToken cancellationToken = default);

    Task<bool> ValidateTwoFactorCodeAsync(Guid userId, string code, CancellationToken cancellationToken = default);
}

namespace SupportDesk.Application.Contracts.Infraestructure.Security;

public interface ITokenGenerator
{
    string GenerateToken(
        TokenGenerationRequest request, 
        CancellationToken cancellationToken = default);

    public string? RefreshToken(
        string token,
        CancellationToken cancellationToken = default);
}

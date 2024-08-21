namespace SupportDesk.Application.Contracts.Infraestructure.Security;

public interface ITokenGenerator
{
    string GenerateToken(TokenGenerationRequest request);

    public string? RefreshToken(string token);
}

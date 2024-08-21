namespace SupportDesk.Application.Contracts.Infraestructure.Security;

public class TokenGenerationRequest
{
    public Guid UserId { get; set; }

    public string Email { get; set; } = string.Empty!;

    public bool IsAdmin { get; set; }

    public Dictionary<string, object> CustomClaims { get; set; } = new();
}

using SupportDesk.Domain.Common;

namespace SupportDesk.Domain.Entities
{
    public class User : AuditableEntity
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty!;
        public string FirstName { get; set; } = string.Empty!;
        public string LastName { get; set; } = string.Empty!;
        public DateTime? BirthDate { get; set; }
        public int? GenderId { get; set; }
        public string? PhotoUrl { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsSupervisor { get; set; }
        public string PasswordHash { get; set; } = string.Empty!;

        public Gender? Gender { get; set; }
        public ICollection<UserZone>? UserZones { get; set; }
        public ICollection<UserRequestType>? UserRequestTypes { get; set; }
        public ICollection<TwoFactorAuthToken>? TwoFactorAuthTokens { get; set; }
        public ICollection<Request>? ReviewedRequests { get; set; } // Requests reviewed by this user
    }
}

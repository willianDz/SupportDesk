namespace SupportDesk.Domain.Entities
{
    public class TwoFactorAuthToken
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; } = string.Empty!;
        public DateTime ExpirationDate { get; set; }
        public bool IsActive { get; set; }

        public User User { get; set; } = null!;
    }
}

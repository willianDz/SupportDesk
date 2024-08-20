using SupportDesk.Domain.Common;

namespace SupportDesk.Domain.Entities
{
    public class UserZone : AuditableEntity
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int ZoneId { get; set; }

        public User User { get; set; } = null!;
        public Zone Zone { get; set; } = null!;
    }
}

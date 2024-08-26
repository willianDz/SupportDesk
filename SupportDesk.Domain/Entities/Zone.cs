using SupportDesk.Domain.Common;

namespace SupportDesk.Domain.Entities
{
    public class Zone : AuditableEntity
    {
        public int ZoneId { get; set; }
        public string Description { get; set; } = string.Empty!;
        public string Abbreviation { get; set; } = string.Empty!;
        public ICollection<UserZone>? UserZones { get; set; }
        public ICollection<Request>? Requests { get; set; }
    }
}

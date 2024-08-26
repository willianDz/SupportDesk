using SupportDesk.Domain.Common;

namespace SupportDesk.Domain.Entities
{
    public class Gender : AuditableEntity
    {
        public int GenderId { get; set; }
        public string Description { get; set; } = string.Empty!;
        public string Abbreviation { get; set; } = string.Empty!;
        public ICollection<User>? Users { get; set; }
    }
}

using SupportDesk.Domain.Common;

namespace SupportDesk.Domain.Entities
{
    public class UserRequestType : AuditableEntity
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int RequestTypeId { get; set; }

        public User User { get; set; } = null!;
        public RequestType RequestType { get; set; } = null!;
    }
}

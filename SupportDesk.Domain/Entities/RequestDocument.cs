using SupportDesk.Domain.Common;

namespace SupportDesk.Domain.Entities
{
    public class RequestDocument : AuditableEntity
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public string DocumentUrl { get; set; } = string.Empty!;

        public Request Request { get; set; } = null!;
    }
}

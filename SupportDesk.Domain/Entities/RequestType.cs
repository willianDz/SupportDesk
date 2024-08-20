using SupportDesk.Domain.Common;

namespace SupportDesk.Domain.Entities
{
    public class RequestType : AuditableEntity
    {
        public int RequestTypeId { get; set; }
        public string Description { get; set; } = string.Empty!;
        public string Abbreviation { get; set; } = string.Empty!;
        public ICollection<UserRequestType>? UserRequestTypes { get; set; }
        public ICollection<Request>? Requests { get; set; }
    }
}

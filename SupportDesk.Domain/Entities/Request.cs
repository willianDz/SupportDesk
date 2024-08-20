using SupportDesk.Domain.Common;

namespace SupportDesk.Domain.Entities
{
    public class Request : AuditableEntity
    {
        public int Id { get; set; }
        public Guid? ReviewerUserId { get; set; }
        public int RequestTypeId { get; set; }
        public int ZoneId { get; set; }
        public string Comments { get; set; } = string.Empty!;
        public DateTime? StartReviewDate { get; set; }
        public DateTime? ApprovalRejectionDate { get; set; }
        public int RequestStatusId { get; set; }

        public User? ReviewerUser { get; set; }
        public RequestType RequestType { get; set; } = null!;
        public Zone Zone { get; set; } = null!;
        public RequestStatus RequestStatus { get; set; } = null!;
        public ICollection<RequestDocument>? RequestDocuments { get; set; }

        public bool IsApprovalDateValid()
        {
            if (ApprovalRejectionDate == null)
                return true;

            return ApprovalRejectionDate >= StartReviewDate;
        }

    }
}

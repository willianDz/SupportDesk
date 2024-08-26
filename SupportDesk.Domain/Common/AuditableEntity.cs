namespace SupportDesk.Domain.Common
{
    public class AuditableEntity
    {
        public bool IsActive { get; set; } = true;
        public Guid? CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public Guid? LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}

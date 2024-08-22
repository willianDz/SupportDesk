
namespace SupportDesk.Application.Models.Dtos;

public class RequestDto
{
    public int Id { get; set; }
    public Guid? ReviewerUserId { get; set; }
    public int RequestTypeId { get; set; }
    public int ZoneId { get; set; }
    public string Comments { get; set; } = string.Empty!;
    public DateTime? StartReviewDate { get; set; }
    public DateTime? ApprovalRejectionDate { get; set; }
    public int RequestStatusId { get; set; }
    public bool IsActive { get; set; } = true;

    public UserDto? ReviewerUser { get; set; }
    public RequestTypeDto RequestType { get; set; } = null!;
    public ZoneDto Zone { get; set; } = null!;
    public RequestStatusDto RequestStatus { get; set; } = null!;
    public ICollection<RequestDocumentDto>? RequestDocuments { get; set; }
}

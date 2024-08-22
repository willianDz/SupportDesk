namespace SupportDesk.Application.Models.Dtos;

public class RequestDocumentDto
{
    public int Id { get; set; }
    public int RequestId { get; set; }
    public string DocumentUrl { get; set; } = string.Empty!;
}

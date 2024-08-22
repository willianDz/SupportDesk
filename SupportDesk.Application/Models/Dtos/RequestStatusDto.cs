namespace SupportDesk.Application.Models.Dtos;

public class RequestStatusDto
{
    public int RequestStatusId { get; set; }
    public string Description { get; set; } = string.Empty!;
    public string Abbreviation { get; set; } = string.Empty!;
}

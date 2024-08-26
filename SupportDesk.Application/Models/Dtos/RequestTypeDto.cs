namespace SupportDesk.Application.Models.Dtos;

public class RequestTypeDto
{
    public int RequestTypeId { get; set; }
    public string Description { get; set; } = string.Empty!;
    public string Abbreviation { get; set; } = string.Empty!;
}

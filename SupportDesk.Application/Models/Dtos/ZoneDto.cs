namespace SupportDesk.Application.Models.Dtos;

public class ZoneDto
{
    public int ZoneId { get; set; }
    public string Description { get; set; } = string.Empty!;
    public string Abbreviation { get; set; } = string.Empty!;
}

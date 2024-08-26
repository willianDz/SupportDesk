namespace SupportDesk.Application.Models.Dtos;

public class GenderDto
{
    public int GenderId { get; set; }
    public string Description { get; set; } = string.Empty!;
    public string Abbreviation { get; set; } = string.Empty!;        
}

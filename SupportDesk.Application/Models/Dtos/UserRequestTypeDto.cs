namespace SupportDesk.Application.Models.Dtos;

public class UserRequestTypeDto
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public int RequestTypeId { get; set; }
}

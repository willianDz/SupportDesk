namespace SupportDesk.Application.Models.Dtos;

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty!;
    public string FirstName { get; set; } = string.Empty!;
    public string LastName { get; set; } = string.Empty!;
    public DateTime? BirthDate { get; set; }
    public int? GenderId { get; set; }
    public string? PhotoUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsAdmin { get; set; }
    public bool IsSupervisor { get; set; }

    public GenderDto? Gender { get; set; }
    public ICollection<UserZoneDto>? UserZones { get; set; }
    public ICollection<UserRequestTypeDto>? UserRequestTypes { get; set; }
    public ICollection<RequestDto>? ReviewedRequests { get; set; }
}

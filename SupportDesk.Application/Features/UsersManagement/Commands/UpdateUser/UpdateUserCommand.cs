using MediatR;
using Microsoft.AspNetCore.Http;

namespace SupportDesk.Application.Features.UsersManagement.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<UpdateUserCommandResponse>
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty!;
    public string FirstName { get; set; } = string.Empty!;
    public string LastName { get; set; } = string.Empty!;
    public string? Password { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsSupervisor { get; set; }
    public IFormFile? Photo { get; set; }
    public bool IsActive { get; set; }
}

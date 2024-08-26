using MediatR;
using Microsoft.AspNetCore.Http;

namespace SupportDesk.Application.Features.UsersManagement.Commands.CreateUser;

public class CreateUserCommand : IRequest<CreateUserCommandResponse>
{
    public string Email { get; set; } = string.Empty!;
    public string FirstName { get; set; } = string.Empty!;
    public string LastName { get; set; } = string.Empty!;
    public string Password { get; set; } = string.Empty!;
    public bool IsAdmin { get; set; }
    public bool IsSupervisor { get; set; }
    public IFormFile? Photo { get; set; }
}
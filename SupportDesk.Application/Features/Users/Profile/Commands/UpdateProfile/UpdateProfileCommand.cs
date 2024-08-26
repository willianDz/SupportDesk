using MediatR;
using Microsoft.AspNetCore.Http;

namespace SupportDesk.Application.Features.Users.Profile.Commands.UpdateProfile;

public class UpdateProfileCommand : IRequest<UpdateProfileCommandResponse>
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty!;
    public string LastName { get; set; } = string.Empty!;
    public DateTime DateOfBirth { get; set; }
    public int GenderId { get; set; }
    public IFormFile? Photo { get; set; }
}

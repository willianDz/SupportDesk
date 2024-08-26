using MediatR;

namespace SupportDesk.Application.Features.UsersManagement.Commands.InactivateUser;

public class InactivateUserCommand : IRequest<InactivateUserCommandResponse>
{
    public Guid UserId { get; set; }
}

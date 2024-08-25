using FluentValidation;
using SupportDesk.Application.Constants;

namespace SupportDesk.Application.Features.UsersManagement.Commands.InactivateUser;

public class InactivateUserCommandValidator : AbstractValidator<InactivateUserCommand>
{
    public InactivateUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage(UsersMessages.InvalidUserId);
    }
}

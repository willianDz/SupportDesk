using FluentValidation;
using SupportDesk.Application.Constants;

namespace SupportDesk.Application.Features.UsersManagement.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty().WithMessage(UsersMessages.InvalidUserId);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage(UsersMessages.InvalidEmail);
        RuleFor(x => x.FirstName).NotEmpty().WithMessage(UsersMessages.InvalidFirstName);
        RuleFor(x => x.LastName).NotEmpty().WithMessage(UsersMessages.InvalidLastName);

        When(x => !string.IsNullOrEmpty(x.Password), () =>
        {
            RuleFor(x => x.Password).MinimumLength(8).WithMessage(UsersMessages.InvalidPassword);
        });
    }
}

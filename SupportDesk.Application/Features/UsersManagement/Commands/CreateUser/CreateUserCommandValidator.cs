using FluentValidation;
using SupportDesk.Application.Constants;

namespace SupportDesk.Application.Features.UsersManagement.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage(UsersMessages.InvalidEmail);
        RuleFor(x => x.FirstName).NotEmpty().WithMessage(UsersMessages.InvalidFirstName);
        RuleFor(x => x.LastName).NotEmpty().WithMessage(UsersMessages.InvalidLastName);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).WithMessage(UsersMessages.InvalidPassword);
    }
}

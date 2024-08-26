using FluentValidation;
using SupportDesk.Application.Constants;

namespace SupportDesk.Application.Features.Users.Profile.Commands.UpdateProfile;

public class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().WithMessage(UsersMessages.InvalidFirstName);
        RuleFor(x => x.LastName).NotEmpty().WithMessage(UsersMessages.InvalidLastName);
        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage(UsersMessages.InvalidDateOfBirth)
            .Must(BeAValidAge).WithMessage(UsersMessages.InvalidAge);
        RuleFor(x => x.GenderId).NotEmpty().WithMessage(UsersMessages.InvalidGender);
    }

    private bool BeAValidAge(DateTime date)
    {
        var age = DateTime.Today.Year - date.Year;
        return age >= 18;  // Ejemplo de validación para mayores de 18 años.
    }
}

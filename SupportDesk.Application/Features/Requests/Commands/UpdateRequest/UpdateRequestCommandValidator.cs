using FluentValidation;

namespace SupportDesk.Application.Features.Requests.Commands.UpdateRequest;

public class UpdateRequestCommandValidator : AbstractValidator<UpdateRequestCommand>
{
    public UpdateRequestCommandValidator()
    {
        RuleFor(x => x.UserId).NotNull().NotEmpty().WithMessage("Usuario inválido.");
        RuleFor(x => x.RequestTypeId).GreaterThan(0).WithMessage("Tipo de solicitud inválida.");
        RuleFor(x => x.ZoneId).GreaterThan(0).WithMessage("Zona inválida.");
        RuleFor(x => x.Comments)
            .NotEmpty().WithMessage("Los comentarios son requeridos.")
            .MinimumLength(15).WithMessage("Los comentarios deben tener al menos 15 caracteres.")
            .MaximumLength(800).WithMessage("Los comentarios no deben tener mas de 800 caracteres.");
    }
}

using FluentValidation;

namespace SupportDesk.Application.Features.Requests.Commands.InactivateRequest;

public class InactivateRequestCommandValidator : AbstractValidator<InactivateRequestCommand>
{
    public InactivateRequestCommandValidator()
    {
        RuleFor(x => x.RequestId).GreaterThan(0).WithMessage("Solicitud ID Inválida.");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("Usuario inválido.");
    }
}

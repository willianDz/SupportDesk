using FluentValidation;
using SupportDesk.Application.Constants;

namespace SupportDesk.Application.Features.Requests.Commands.InactivateRequest;

public class InactivateRequestCommandValidator : AbstractValidator<InactivateRequestCommand>
{
    public InactivateRequestCommandValidator()
    {
        RuleFor(x => x.RequestId).GreaterThan(0).WithMessage(RequestMessages.InvalidRequestId);
        RuleFor(x => x.UserId).NotEmpty().WithMessage(RequestMessages.InvalidUser);
    }
}

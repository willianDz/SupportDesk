namespace SupportDesk.Application.Features.Requests.Commands.CreateRequest;

using FluentValidation;
using SupportDesk.Application.Constants;

public class CreateRequestCommandValidator : AbstractValidator<CreateRequestCommand>
{
    public CreateRequestCommandValidator()
    {
        RuleFor(x => x.UserId).NotNull().NotEmpty().WithMessage(RequestMessages.InvalidUser);
        RuleFor(x => x.RequestTypeId).GreaterThan(0).WithMessage(RequestMessages.InvalidRequestType);
        RuleFor(x => x.ZoneId).GreaterThan(0).WithMessage(RequestMessages.InvalidZone);
        RuleFor(x => x.Comments)
            .NotEmpty().WithMessage(RequestMessages.RequiredComments)
            .MinimumLength(15).WithMessage(RequestMessages.CommentsMinLenght)
            .MaximumLength(800).WithMessage(RequestMessages.CommentsMaxLenght);
    }
}

using FluentValidation;

namespace ClaimsModule.Application.Claims.Commands.AssignHandler;

public sealed class AssignHandlerCommandValidator : AbstractValidator<AssignHandlerCommand>
{
    public AssignHandlerCommandValidator()
    {
        RuleFor(x => x.ClaimId)
            .NotEmpty();

        RuleFor(x => x.HandlerId)
            .NotEmpty();
    }
}
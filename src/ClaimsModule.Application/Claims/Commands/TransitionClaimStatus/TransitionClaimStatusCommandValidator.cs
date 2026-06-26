using ClaimsModule.Domain.Enums;
using FluentValidation;

namespace ClaimsModule.Application.Claims.Commands.TransitionClaimStatus;

public sealed class TransitionClaimStatusCommandValidator : AbstractValidator<TransitionClaimStatusCommand>
{
    public TransitionClaimStatusCommandValidator()
    {
        RuleFor(x => x.ClaimId)
            .NotEmpty();

        RuleFor(x => x.RowVersion)
            .NotEmpty();

        RuleFor(x => x.TargetStatus)
            .IsInEnum();

        RuleFor(x => x.Reason)
            .NotEmpty()
            .When(x => x.TargetStatus is ClaimStatus.Withdrawn or ClaimStatus.Reopened)
            .WithMessage("Reason is required for Withdrawn and Reopened transitions.");
    }
}
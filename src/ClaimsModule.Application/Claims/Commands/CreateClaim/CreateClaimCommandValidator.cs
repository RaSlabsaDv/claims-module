using FluentValidation;

namespace ClaimsModule.Application.Claims.Commands.CreateClaim;

public sealed class CreateClaimCommandValidator : AbstractValidator<CreateClaimCommand>
{
    public CreateClaimCommandValidator()
    {
        RuleFor(x => x.OrganisationId)
            .NotEmpty();

        RuleFor(x => x.Severity)
            .IsInEnum();

        RuleFor(x => x.LossDate)
            .NotEmpty()
            .LessThanOrEqualTo(DateTimeOffset.UtcNow)
            .WithMessage("Loss date cannot be in the future.");

        RuleFor(x => x.LossDescription)
            .NotEmpty()
            .MinimumLength(20)
            .MaximumLength(5000);

        RuleFor(x => x.CauseOfLossCode)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.EstimatedLossAmount)
            .GreaterThan(0)
            .When(x => x.EstimatedLossAmount.HasValue);

        RuleFor(x => x.PolicyNumber)
            .NotEmpty()
            .When(x => x.PolicyId.HasValue)
            .WithMessage("PolicyNumber is required when PolicyId is provided.");

        RuleFor(x => x.ClientName)
            .NotEmpty()
            .When(x => x.PolicyId.HasValue)
            .WithMessage("ClientName is required when PolicyId is provided.");
    }
}
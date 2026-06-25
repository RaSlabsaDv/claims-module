using FluentValidation;

namespace ClaimsModule.Application.Reserves.Commands.AdjustReserve;

public sealed class AdjustReserveCommandValidator : AbstractValidator<AdjustReserveCommand>
{
    public AdjustReserveCommandValidator()
    {
        RuleFor(x => x.ReserveId)
            .NotEmpty();

        RuleFor(x => x.Amount)
            .NotEqual(0)
            .WithMessage("Adjustment amount cannot be zero.");

        RuleFor(x => x.ChangeReason)
            .NotEmpty()
            .MaximumLength(500);
    }
}
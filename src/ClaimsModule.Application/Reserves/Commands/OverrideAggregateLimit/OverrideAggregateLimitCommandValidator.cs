using FluentValidation;

namespace ClaimsModule.Application.Reserves.Commands.OverrideAggregateLimit;

public sealed class OverrideAggregateLimitCommandValidator : AbstractValidator<OverrideAggregateLimitCommand>
{
    public OverrideAggregateLimitCommandValidator()
    {
        RuleFor(x => x.ReserveId)
            .NotEmpty();
    }
}
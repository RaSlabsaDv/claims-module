using FluentValidation;

namespace ClaimsModule.Application.Reserves.Commands.RejectReserve;

public sealed class RejectReserveCommandValidator : AbstractValidator<RejectReserveCommand>
{
    public RejectReserveCommandValidator()
    {
        RuleFor(x => x.ReserveId)
            .NotEmpty();

        RuleFor(x => x.TransactionId)
            .NotEmpty();

        RuleFor(x => x.RejectionReason)
            .NotEmpty()
            .MaximumLength(500);
    }
}
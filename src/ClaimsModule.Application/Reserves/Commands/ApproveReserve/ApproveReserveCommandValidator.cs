using FluentValidation;

namespace ClaimsModule.Application.Reserves.Commands.ApproveReserve;

public sealed class ApproveReserveCommandValidator : AbstractValidator<ApproveReserveCommand>
{
    public ApproveReserveCommandValidator()
    {
        RuleFor(x => x.ReserveId)
            .NotEmpty();

        RuleFor(x => x.TransactionId)
            .NotEmpty();
    }
}
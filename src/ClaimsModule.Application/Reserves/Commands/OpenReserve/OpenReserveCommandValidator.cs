using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Enums;
using FluentValidation;

namespace ClaimsModule.Application.Reserves.Commands.OpenReserve;

public sealed class OpenReserveCommandValidator : AbstractValidator<OpenReserveCommand>
{
    public OpenReserveCommandValidator(IReserveRepository reserveRepository)
    {
        RuleFor(x => x.ClaimId)
            .NotEmpty();

        RuleFor(x => x.ComponentType)
            .IsInEnum();

        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .When(x => x.ComponentType != ReserveComponentType.SubrogationRecoverable)
            .WithMessage("Amount must be greater than zero.");

        RuleFor(x => x.ChangeReason)
            .NotEmpty()
            .MaximumLength(500);
    }
}
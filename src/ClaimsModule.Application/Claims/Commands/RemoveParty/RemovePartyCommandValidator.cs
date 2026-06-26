using FluentValidation;

namespace ClaimsModule.Application.Claims.Commands.RemoveParty;

public sealed class RemovePartyCommandValidator : AbstractValidator<RemovePartyCommand>
{
    public RemovePartyCommandValidator()
    {
        RuleFor(x => x.ClaimId)
            .NotEmpty();

        RuleFor(x => x.PartyId)
            .NotEmpty();
    }
}
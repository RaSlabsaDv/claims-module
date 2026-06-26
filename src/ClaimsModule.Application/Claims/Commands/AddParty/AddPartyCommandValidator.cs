using ClaimsModule.Domain.Enums;
using FluentValidation;

namespace ClaimsModule.Application.Claims.Commands.AddParty;

public sealed class AddPartyCommandValidator : AbstractValidator<AddPartyCommand>
{
    public AddPartyCommandValidator()
    {
        RuleFor(x => x.ClaimId)
            .NotEmpty();

        RuleFor(x => x.PartyRole)
            .IsInEnum();

        RuleFor(x => x.PartyType)
            .IsInEnum();

        // Person — потрібне ім'я або прізвище
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .When(x => x.PartyType == PartyType.Person)
            .WithMessage("FirstName is required for Person party type.");

        // Company — потрібна назва
        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .When(x => x.PartyType == PartyType.Company)
            .WithMessage("CompanyName is required for Company party type.");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
    }
}
using FluentValidation;

namespace ClaimsModule.Application.Claims.Commands.AddRiskObject;

public sealed class AddRiskObjectCommandValidator : AbstractValidator<AddRiskObjectCommand>
{
    public AddRiskObjectCommandValidator()
    {
        RuleFor(x => x.ClaimId)
            .NotEmpty();

        RuleFor(x => x.AssetType)
            .IsInEnum();

        RuleFor(x => x.AssetDescription)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.DamageDescription)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrWhiteSpace(x.DamageDescription));
    }
}
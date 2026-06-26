using FluentValidation;

namespace ClaimsModule.Application.Claims.Commands.UpdateNotes;

public sealed class UpdateNotesCommandValidator : AbstractValidator<UpdateNotesCommand>
{
    public UpdateNotesCommandValidator()
    {
        RuleFor(x => x.ClaimId)
            .NotEmpty();

        RuleFor(x => x.Notes)
            .NotEmpty()
            .MaximumLength(5000);
    }
}
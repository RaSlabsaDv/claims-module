using ClaimsModule.Domain.Constants;
using FluentValidation;

namespace ClaimsModule.Application.Claims.Commands.UploadDocument;

public sealed class UploadDocumentCommandValidator : AbstractValidator<UploadDocumentCommand>
{
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB

    public UploadDocumentCommandValidator()
    {
        RuleFor(x => x.ClaimId)
            .NotEmpty();

        RuleFor(x => x.RowVersion)
            .NotEmpty();

        RuleFor(x => x.DocumentType)
            .NotEmpty()
            .Must(t => DocumentTypes.All.Contains(t))
            .WithMessage("Invalid document type.");

        RuleFor(x => x.DocumentName)
            .NotEmpty()
            .MaximumLength(255);

        RuleFor(x => x.ContentType)
            .NotEmpty();

        RuleFor(x => x.FileSizeBytes)
            .GreaterThan(0)
            .LessThanOrEqualTo(MaxFileSizeBytes)
            .WithMessage("File size cannot exceed 10MB.");

        RuleFor(x => x.Content)
            .NotNull();
    }
}
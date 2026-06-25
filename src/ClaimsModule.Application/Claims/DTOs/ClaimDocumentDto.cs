namespace ClaimsModule.Application.Claims.Dtos;

public sealed record ClaimDocumentDto(
    Guid Id,
    string DocumentType,
    string DocumentName,
    string ContentType,
    long FileSizeBytes,
    DateTimeOffset UploadedAt,
    Guid? UploadedByUserId,
    string? Notes,
    string DownloadUrl  // SAS URL — генерується в handler
);
using ClaimsModule.Domain.Common;

namespace ClaimsModule.Domain.Entities;

public sealed class ClaimDocument : BaseEntity
{
    public Guid ClaimId { get; private set; }
    public Claim Claim { get; private set; } = default!;

    public string DocumentType { get; private set; } = default!;
    public string DocumentName { get; private set; } = default!;
    public string BlobPath { get; private set; } = default!;
    public string ContentType { get; private set; } = default!;
    public long FileSizeBytes { get; private set; }
    public DateTimeOffset UploadedAt { get; private set; }
    public Guid? UploadedByUserId { get; private set; }
    public string? Notes { get; private set; }

    // EF Core
    private ClaimDocument() { }

    public static ClaimDocument Create(
        Guid claimId,
        string documentType,
        string documentName,
        string blobPath,
        string contentType,
        long fileSizeBytes,
        Guid uploadedByUserId,
        string? notes = null)
    {
        var document = new ClaimDocument
        {
            ClaimId = claimId,
            DocumentType = documentType,
            DocumentName = documentName,
            BlobPath = blobPath,
            ContentType = contentType,
            FileSizeBytes = fileSizeBytes,
            UploadedAt = DateTimeOffset.UtcNow,
            UploadedByUserId = uploadedByUserId,
            Notes = notes
        };

        document.SetCreated(uploadedByUserId);

        return document;
    }
}

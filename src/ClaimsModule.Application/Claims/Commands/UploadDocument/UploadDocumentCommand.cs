using MediatR;

namespace ClaimsModule.Application.Claims.Commands.UploadDocument;

public sealed record UploadDocumentCommand(
    Guid ClaimId,
    byte[] RowVersion,
    string DocumentType,
    string DocumentName,
    string ContentType,
    long FileSizeBytes,
    Stream Content) : IRequest<Guid>;
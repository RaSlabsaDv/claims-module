using ClaimsModule.Application.Claims.Dtos;
using ClaimsModule.Application.Common.Interfaces;
using MediatR;

namespace ClaimsModule.Application.Claims.Queries.GetClaimDocuments;

public sealed class GetClaimDocumentsQueryHandler
(
    IClaimDocumentRepository claimDocumentRepository,
    IStorageService storageService
) : IRequestHandler<GetClaimDocumentsQuery, IReadOnlyList<ClaimDocumentDto>>
{
    public async Task<IReadOnlyList<ClaimDocumentDto>> Handle(GetClaimDocumentsQuery request, CancellationToken ct)
    {
        var documents = await claimDocumentRepository.ListByClaimAsync(request.ClaimId, ct);

        var dtos = await Task.WhenAll(documents.Select(async doc => new ClaimDocumentDto(
            Id: doc.Id,
            DocumentType: doc.DocumentType,
            DocumentName: doc.DocumentName,
            ContentType: doc.ContentType,
            FileSizeBytes: doc.FileSizeBytes,
            UploadedAt: doc.UploadedAt,
            UploadedByUserId: doc.UploadedByUserId,
            Notes: doc.Notes,
            DownloadUrl: await storageService.GetDownloadUrlAsync(doc.BlobPath, ct))));

        return dtos;
    }
}
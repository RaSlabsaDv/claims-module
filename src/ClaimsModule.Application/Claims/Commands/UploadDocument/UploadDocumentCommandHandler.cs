using ClaimsModule.Application.Common.Exceptions;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Constants;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Exceptions;
using MediatR;

namespace ClaimsModule.Application.Claims.Commands.UploadDocument;

public sealed class UploadDocumentCommandHandler(
    IClaimRepository claimRepository,
    IStorageService storageService,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IAuditLogService auditLog)
    : IRequestHandler<UploadDocumentCommand, Guid>
{
    public async Task<Guid> Handle(UploadDocumentCommand request, CancellationToken ct)
    {
        var userId = currentUser.UserId ?? throw new UnauthorizedException();

        var claim = await claimRepository.GetByIdAsync(request.ClaimId, ct)
            ?? throw new NotFoundException(nameof(Claim), request.ClaimId);

        claimRepository.SetOriginalRowVersion(claim, request.RowVersion);

        var blobPath = $"claims/{request.ClaimId}/documents/{Guid.NewGuid()}/{request.DocumentName}";

        var document = ClaimDocument.Create(
            claimId: request.ClaimId,
            documentType: request.DocumentType,
            documentName: request.DocumentName,
            blobPath: blobPath,
            contentType: request.ContentType,
            fileSizeBytes: request.FileSizeBytes,
            uploadedByUserId: userId);

        // Спочатку завантажуємо файл
        await storageService.UploadAsync(blobPath, request.Content, request.ContentType, ct);

        try
        {
            claim.AddDocument(document);
            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception)
        {
            // Якщо БД не зберегла — видаляємо файл з storage
            await storageService.DeleteAsync(blobPath, ct);
            throw;
        }

        await auditLog.LogAsync(
            claimId: request.ClaimId,
            eventType: AuditEventTypes.DocumentUploaded,
            description: $"Document '{request.DocumentName}' ({request.DocumentType}) uploaded.",
            relatedEntityId: document.Id,
            relatedEntityType: nameof(ClaimDocument),
            newValue: new
            {
                request.DocumentName,
                request.DocumentType,
                request.ContentType,
                request.FileSizeBytes
            });

        return document.Id;
    }
}
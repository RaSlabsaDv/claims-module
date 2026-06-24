using System.Text.Json;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;

namespace ClaimsModule.Infrastructure.Services;

public sealed class AuditLogService(
    IUnitOfWork unitOfWork,
    IClaimAuditLogWriter auditLogWriter,
    ICurrentUserService currentUser) : IAuditLogService
{
    public async Task LogAsync(
        Guid claimId,
        string eventType,
        string description,
        CancellationToken ct = default,
        Guid? relatedEntityId = null,
        string? relatedEntityType = null,
        object? oldValue = null,
        object? newValue = null)
    {
        var entry = ClaimAuditLog.Create(
            claimId: claimId,
            eventType: eventType,
            description: description,
            createdByUserId: currentUser.UserId == Guid.Empty ? null : currentUser.UserId,
            oldValue: oldValue is null ? null : JsonSerializer.Serialize(oldValue),
            newValue: newValue is null ? null : JsonSerializer.Serialize(newValue),
            relatedEntityId: relatedEntityId,
            relatedEntityType: relatedEntityType,
            correlationId: currentUser.CorrelationId);

        await auditLogWriter.AddAsync(entry, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
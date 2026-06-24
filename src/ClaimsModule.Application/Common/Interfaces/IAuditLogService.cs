namespace ClaimsModule.Application.Common.Interfaces;

public interface IAuditLogService
{
    Task LogAsync(
        Guid claimId,
        string eventType,
        string description,
        CancellationToken ct = default,
        Guid? relatedEntityId = null,
        string? relatedEntityType = null,
        object? oldValue = null,
        object? newValue = null);
}

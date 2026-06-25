namespace ClaimsModule.Application.Claims.Queries.GetClaimAuditLog;

public sealed record AuditLogEntryDto(
    Guid Id,
    string EventType,
    string Description,
    string? OldValue,
    string? NewValue,
    Guid? RelatedEntityId,
    string? RelatedEntityType,
    Guid? CorrelationId,
    DateTimeOffset CreatedAt,
    Guid? CreatedByUserId
);
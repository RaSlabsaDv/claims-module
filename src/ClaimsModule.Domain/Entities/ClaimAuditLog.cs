namespace ClaimsModule.Domain.Entities;

// Immutable append-only — не наслідує BaseEntity
// Без UpdatedAt, без IsDeleted, без UserModified (BR-A-01)
public sealed class ClaimAuditLog
{
    public Guid Id { get; private set; }
    public Guid ClaimId { get; private set; }
    public Claim Claim { get; private set; } = default!;

    public string EventType { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string? OldValue { get; private set; }   // JSON
    public string? NewValue { get; private set; }   // JSON
    public Guid? RelatedEntityId { get; private set; }
    public string? RelatedEntityType { get; private set; }
    public Guid? CorrelationId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public Guid? CreatedByUserId { get; private set; }

    // EF Core
    private ClaimAuditLog() { }

    public static ClaimAuditLog Create(
        Guid claimId,
        string eventType,
        string description,
        Guid? createdByUserId = null,
        string? oldValue = null,
        string? newValue = null,
        Guid? relatedEntityId = null,
        string? relatedEntityType = null,
        Guid? correlationId = null)
    {
        return new ClaimAuditLog
        {
            Id = Guid.NewGuid(),
            ClaimId = claimId,
            EventType = eventType,
            Description = description,
            CreatedByUserId = createdByUserId,
            OldValue = oldValue,
            NewValue = newValue,
            RelatedEntityId = relatedEntityId,
            RelatedEntityType = relatedEntityType,
            CorrelationId = correlationId,
            CreatedAt = DateTimeOffset.UtcNow
        };
    }
}

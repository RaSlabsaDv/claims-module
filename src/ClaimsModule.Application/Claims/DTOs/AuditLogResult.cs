namespace ClaimsModule.Application.Claims.Queries.GetClaimAuditLog;

public sealed record AuditLogResult(
    IReadOnlyList<AuditLogEntryDto> Items,
    int TotalCount);
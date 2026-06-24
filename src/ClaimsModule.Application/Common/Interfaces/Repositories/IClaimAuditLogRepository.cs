using ClaimsModule.Domain.Entities;

namespace ClaimsModule.Application.Common.Interfaces;

public interface IClaimAuditLogRepository
{
    // Read-only — всі записи виключно через IAuditLogService
    Task<(IReadOnlyList<ClaimAuditLog> Items, int TotalCount)> ListByClaimAsync(
        Guid claimId,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task<ClaimAuditLog?> GetLastByEventTypeAsync(
        Guid claimId,
        string eventType,
        CancellationToken ct = default);
}

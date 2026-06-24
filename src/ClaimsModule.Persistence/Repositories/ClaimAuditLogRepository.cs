using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Persistence.Repositories;

public sealed class ClaimAuditLogRepository(ClaimsDbContext context) : IClaimAuditLogRepository
{
    public async Task<ClaimAuditLog?> GetLastByEventTypeAsync(
        Guid claimId,
        string eventType,
        CancellationToken ct = default)
    {
        return await context.ClaimAuditLogs
            .AsNoTracking()
            .Where(a => a.ClaimId == claimId && a.EventType == eventType)
            .OrderByDescending(a => a.CreatedAt)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<(IReadOnlyList<ClaimAuditLog> Items, int TotalCount)> ListByClaimAsync(
        Guid claimId,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = context.ClaimAuditLogs
            .Where(a => a.ClaimId == claimId)
            .OrderByDescending(a => a.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }
}
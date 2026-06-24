using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;

namespace ClaimsModule.Persistence.Repositories;

public sealed class ClaimAuditLogWriter(ClaimsDbContext context) : IClaimAuditLogWriter
{
    public async Task AddAsync(ClaimAuditLog entry, CancellationToken ct = default)
    {
        await context.ClaimAuditLogs.AddAsync(entry, ct);
    }
}
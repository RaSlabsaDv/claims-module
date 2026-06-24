using ClaimsModule.Domain.Entities;

namespace ClaimsModule.Application.Common.Interfaces;

public interface IClaimAuditLogWriter
{
    Task AddAsync(ClaimAuditLog entry, CancellationToken ct = default);
}
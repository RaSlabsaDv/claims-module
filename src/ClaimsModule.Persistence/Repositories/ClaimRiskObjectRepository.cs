using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Persistence.Repositories;

public sealed class ClaimRiskObjectRepository(ClaimsDbContext context) : IClaimRiskObjectRepository
{
    public async Task<ClaimRiskObject?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.ClaimRiskObjects
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<IReadOnlyList<ClaimRiskObject>> ListByClaimAsync(Guid claimId, CancellationToken ct = default)
        => await context.ClaimRiskObjects
            .AsNoTracking()
            .Where(r => r.ClaimId == claimId)
            .ToListAsync(ct);

    public async Task AddAsync(ClaimRiskObject riskObject, CancellationToken ct = default)
        => await context.ClaimRiskObjects.AddAsync(riskObject, ct);
}
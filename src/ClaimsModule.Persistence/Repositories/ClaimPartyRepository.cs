using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Persistence.Repositories;

public sealed class ClaimPartyRepository(ClaimsDbContext context) : IClaimPartyRepository
{
    public async Task<ClaimParty?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.ClaimParties.FirstOrDefaultAsync(cp => cp.Id == id, ct);
    }

    public async Task<IReadOnlyList<ClaimParty>> ListByClaimAsync(Guid claimId, CancellationToken ct = default)
    {
        return await context.ClaimParties
            .AsNoTracking()
            .Where(cp => cp.ClaimId == claimId)
            .ToListAsync(ct);
    }
}
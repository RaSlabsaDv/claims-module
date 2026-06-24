using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Persistence.Repositories;

public sealed class LossEventRepository(ClaimsDbContext context) : ILossEventRepository
{
    public async Task AddAsync(LossEvent lossEvent, CancellationToken ct = default)
    {
        await context.LossEvents.AddAsync(lossEvent, ct);
    }

    public async Task<LossEvent?> GetByClaimIdAsync(Guid claimId, CancellationToken ct = default)
    {
        return await context.LossEvents.FirstOrDefaultAsync(le => le.ClaimId == claimId, ct);
    }
}
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Persistence.Repositories;

public sealed class CauseOfLossCodeRepository(ClaimsDbContext context) : ICauseOfLossCodeRepository
{
    public async Task<bool> ExistsAsync(string code, CancellationToken ct = default)
    {
        return await context.CauseOfLossCodes
            .AnyAsync(x => x.Code == code, ct);
    }

    public async Task<CauseOfLossCode?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        return await context.CauseOfLossCodes
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Code == code, ct);
    }

    public async Task<IReadOnlyList<CauseOfLossCode>> ListActiveAsync(CancellationToken ct = default)
    {
        return await context.CauseOfLossCodes
            .AsNoTracking()
            .Where(x => x.IsActive)
            .ToListAsync(ct);
    }
}
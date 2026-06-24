using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Persistence.Repositories;

public sealed class ReserveRepository(ClaimsDbContext context) : IReserveRepository
{
    public async Task<ClaimReserveComponent?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.ClaimReserveComponents
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<ClaimReserveComponent?> GetByIdWithTransactionsAsync(Guid id, CancellationToken ct = default)
        => await context.ClaimReserveComponents
            .Include(r => r.Transactions)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<IReadOnlyList<ClaimReserveComponent>> ListByClaimAsync(Guid claimId, CancellationToken ct = default)
        => await context.ClaimReserveComponents
            .AsNoTracking()
            .Include(r => r.Transactions)
            .Where(r => r.ClaimId == claimId)
            .ToListAsync(ct);

    public async Task<ReserveHistory?> GetTransactionByIdAsync(Guid transactionId, CancellationToken ct = default)
        => await context.ReserveHistories
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == transactionId, ct);

    public async Task AddAsync(ClaimReserveComponent reserve, CancellationToken ct = default)
        => await context.ClaimReserveComponents.AddAsync(reserve, ct);

    public async Task<decimal> GetAggregateTotalByClaimAsync(Guid claimId, CancellationToken ct = default)
        => await context.ClaimReserveComponents
            .AsNoTracking()
            .Where(r => r.ClaimId == claimId && r.Status == ReserveComponentStatus.Active)
            .SumAsync(r => r.CurrentAmount, ct);
}
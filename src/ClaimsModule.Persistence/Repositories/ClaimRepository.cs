using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Persistence.Repositories;

public sealed class ClaimRepository(ClaimsDbContext context) : IClaimRepository
{
    public async Task<Claim?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await context.Claims
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<Claim?> GetByIdWithAllAsync(Guid id, CancellationToken ct = default)
        => await context.Claims
            .Include(c => c.LossEvent)
            .Include(c => c.Parties)
            .Include(c => c.RiskObjects)
            .Include(c => c.Reserves)
                .ThenInclude(r => r.Transactions)
            .Include(c => c.Documents)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<(IReadOnlyList<Claim> Items, int TotalCount)> ListAsync(
        ClaimListFilter filter,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        var query = context.Claims
            .AsNoTracking()
            .Include(c => c.LossEvent)
            .AsQueryable();

        if (filter.Status.HasValue)
            query = query.Where(c => c.Status == filter.Status.Value);

        if (filter.DateFrom.HasValue)
            query = query.Where(c => c.ReportedDate >= filter.DateFrom.Value);

        if (filter.DateTo.HasValue)
            query = query.Where(c => c.ReportedDate <= filter.DateTo.Value);

        if (filter.AssignedHandlerId.HasValue)
            query = query.Where(c => c.AssignedHandlerId == filter.AssignedHandlerId.Value);

        if (!string.IsNullOrWhiteSpace(filter.CauseOfLossCode))
            query = query.Where(c => c.LossEvent != null
                && c.LossEvent.CauseOfLossCode == filter.CauseOfLossCode);

        if (filter.PolicyId.HasValue)
            query = query.Where(c => c.PolicyId == filter.PolicyId.Value);

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(c =>
                c.ClaimNumber.Contains(filter.Search) ||
                (c.ClientName != null && c.ClientName.Contains(filter.Search)));

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(c => c.ReportedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task AddAsync(Claim claim, CancellationToken ct = default)
        => await context.Claims.AddAsync(claim, ct);

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        => await context.Claims
            .AsNoTracking()
            .AnyAsync(c => c.Id == id, ct);
}
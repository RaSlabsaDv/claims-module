using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Application.Common.Interfaces;

public interface IClaimRepository
{
    Task<Claim?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Claim?> GetByIdWithAllAsync(Guid id, CancellationToken ct = default);
    Task<(IReadOnlyList<Claim> Items, int TotalCount)> ListAsync(
        ClaimListFilter filter,
        int page,
        int pageSize,
        CancellationToken ct = default);

    Task AddAsync(Claim claim, CancellationToken ct = default);

    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}

public sealed record ClaimListFilter(
    ClaimStatus? Status = null,
    DateTimeOffset? DateFrom = null,
    DateTimeOffset? DateTo = null,
    Guid? AssignedHandlerId = null,
    string? CauseOfLossCode = null,
    Guid? PolicyId = null,
    string? Search = null);  // partial claim number або client name

using ClaimsModule.Domain.Entities;

namespace ClaimsModule.Application.Common.Interfaces;

public interface IClaimPartyRepository
{
    Task<ClaimParty?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ClaimParty>> ListByClaimAsync(Guid claimId, CancellationToken ct = default);
    Task AddAsync(ClaimParty party, CancellationToken ct = default);
}

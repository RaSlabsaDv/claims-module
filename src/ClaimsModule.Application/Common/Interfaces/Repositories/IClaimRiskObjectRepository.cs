using ClaimsModule.Domain.Entities;

namespace ClaimsModule.Application.Common.Interfaces;

public interface IClaimRiskObjectRepository
{
    Task<ClaimRiskObject?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ClaimRiskObject>> ListByClaimAsync(Guid claimId, CancellationToken ct = default);
    Task AddAsync(ClaimRiskObject riskObject, CancellationToken ct = default);
}

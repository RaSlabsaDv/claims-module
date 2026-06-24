using ClaimsModule.Domain.Entities;

namespace ClaimsModule.Application.Common.Interfaces;

public interface IClaimDocumentRepository
{
    Task<ClaimDocument?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ClaimDocument>> ListByClaimAsync(Guid claimId, CancellationToken ct = default);
    Task AddAsync(ClaimDocument document, CancellationToken ct = default);
}

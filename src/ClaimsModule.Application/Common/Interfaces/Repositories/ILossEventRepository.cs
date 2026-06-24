using ClaimsModule.Domain.Entities;

namespace ClaimsModule.Application.Common.Interfaces;

public interface ILossEventRepository
{
    Task<LossEvent?> GetByClaimIdAsync(Guid claimId, CancellationToken ct = default);
    Task AddAsync(LossEvent lossEvent, CancellationToken ct = default);
}

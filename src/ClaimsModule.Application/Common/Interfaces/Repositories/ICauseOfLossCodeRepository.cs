using ClaimsModule.Domain.Entities;

namespace ClaimsModule.Application.Common.Interfaces;

public interface ICauseOfLossCodeRepository
{
    Task<CauseOfLossCode?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<IReadOnlyList<CauseOfLossCode>> ListActiveAsync(CancellationToken ct = default);
}

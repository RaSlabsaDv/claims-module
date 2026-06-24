using ClaimsModule.Domain.Entities;

namespace ClaimsModule.Application.Common.Interfaces;

public interface IPolicyRepository
{
    Task<Policy?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Policy?> GetByPolicyNumberAsync(string policyNumber, CancellationToken ct = default);

    Task<IReadOnlyList<Policy>> SearchAsync(
        string searchTerm,
        int maxResults = 10,
        CancellationToken ct = default);
}

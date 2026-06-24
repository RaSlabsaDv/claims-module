using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Persistence.Repositories;

public sealed class PolicyRepository(ClaimsDbContext context) : IPolicyRepository
{
    public async Task<Policy?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Policies.FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<Policy?> GetByPolicyNumberAsync(string policyNumber, CancellationToken ct = default)
    {
        return await context.Policies.FirstOrDefaultAsync(p => p.PolicyNumber == policyNumber, ct);
    }

    public async Task<IReadOnlyList<Policy>> SearchAsync
    (
        string searchTerm, 
        int maxResults = 10, 
        CancellationToken ct = default)
    {
        return await context.Policies
            .AsNoTracking()
            .Where(p => p.PolicyNumber.Contains(searchTerm) ||
                        p.ClientName.Contains(searchTerm))
            .OrderBy(p => p.PolicyNumber)
            .Take(maxResults)
            .ToListAsync(ct);
    }
}
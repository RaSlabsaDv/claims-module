using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Persistence.Repositories;

public sealed class ClaimDocumentRepository(ClaimsDbContext context) : IClaimDocumentRepository
{
    public async Task AddAsync(ClaimDocument document, CancellationToken ct = default)
    {
        await context.ClaimDocuments.AddAsync(document, ct);
    }

    public async Task<ClaimDocument?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.ClaimDocuments.FirstOrDefaultAsync(cd => cd.Id == id, ct);
    }

    public async Task<IReadOnlyList<ClaimDocument>> ListByClaimAsync(Guid claimId, CancellationToken ct = default)
    {
        return await context.ClaimDocuments
            .AsNoTracking()
            .Where(cd => cd.ClaimId == claimId)
            .ToListAsync(ct);
    }
}
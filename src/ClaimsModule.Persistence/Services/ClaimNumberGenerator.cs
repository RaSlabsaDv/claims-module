using ClaimsModule.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Persistence.Services;

public sealed class ClaimNumberGenerator(ClaimsDbContext context) : IClaimNumberGenerator
{
    public async Task<string> GenerateAsync(Guid organisationId, CancellationToken ct = default)
    {
        var year = DateTime.UtcNow.Year;

        await context.Database.ExecuteSqlRawAsync(
            """
            MERGE ClaimSequences AS target
            USING (SELECT {0} AS OrganisationId, {1} AS Year) AS source
                ON target.OrganisationId = source.OrganisationId
                AND target.Year = source.Year
            WHEN MATCHED THEN
                UPDATE SET LastSequence = target.LastSequence + 1
            WHEN NOT MATCHED THEN
                INSERT (OrganisationId, Year, LastSequence)
                VALUES (source.OrganisationId, source.Year, 1);
            """,
            organisationId, year);

        var lastSequence = await context.ClaimSequences
            .Where(s => s.OrganisationId == organisationId && s.Year == year)
            .Select(s => s.LastSequence)
            .FirstAsync(ct);

        return $"CLM-{year}-{lastSequence:D7}";
    }
}
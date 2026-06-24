namespace ClaimsModule.Domain.Entities;

// Технічна сутність для атомарної генерації ClaimNumber
// Composite PK: OrganisationId + Year
// Оновлюється через raw SQL UPDATE...OUTPUT щоб уникнути race condition
public sealed class ClaimSequence
{
    public Guid OrganisationId { get; private set; }
    public int Year { get; private set; }
    public long LastSequence { get; private set; }

    // EF Core
    private ClaimSequence() { }
}

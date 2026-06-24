using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Domain.Entities;

// Simulated policy — без BaseEntity (seed-only, read-only в рамках assessment)
public sealed class Policy
{
    public Guid Id { get; private set; }
    public string PolicyNumber { get; private set; } = default!;
    public string ClientName { get; private set; } = default!;
    public DateOnly EffectiveDate { get; private set; }
    public DateOnly ExpirationDate { get; private set; }
    public PolicyStatus Status { get; private set; }

    // JSON масив типів покриття (специфікація 9.10)
    public string CoverageTypes { get; private set; } = default!;

    // EF Core
    private Policy() { }

    public bool IsActiveOn(DateOnly date)
        => Status == PolicyStatus.Active
           && date >= EffectiveDate
           && date <= ExpirationDate;
}

using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public sealed class PolicyConfiguration : IEntityTypeConfiguration<Policy>
{
    public void Configure(EntityTypeBuilder<Policy> builder)
    {
        builder.ToTable("Policies");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(e => e.PolicyNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(e => e.PolicyNumber)
            .IsUnique();

        builder.Property(e => e.ClientName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.EffectiveDate)
            .IsRequired();

        builder.Property(e => e.ExpirationDate)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired();

        builder.Property(e => e.CoverageTypes)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        // Seed data (специфікація Section 5.5)
        builder.HasData(
            new { Id = new Guid("20000000-0000-0000-0000-000000000001"), PolicyNumber = "POL-2024-001", ClientName = "Acme Corporation",       EffectiveDate = new DateOnly(2024, 1, 1),  ExpirationDate = new DateOnly(2026, 12, 31), Status = PolicyStatus.Active,    CoverageTypes = "[\"Property\",\"Liability\"]" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000002"), PolicyNumber = "POL-2024-002", ClientName = "John Smith",             EffectiveDate = new DateOnly(2024, 3, 15), ExpirationDate = new DateOnly(2026, 3, 14),  Status = PolicyStatus.Active,    CoverageTypes = "[\"Auto\"]" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000003"), PolicyNumber = "POL-2023-089", ClientName = "Global Logistics Ltd",   EffectiveDate = new DateOnly(2023, 6, 1),  ExpirationDate = new DateOnly(2025, 5, 31),  Status = PolicyStatus.Expired,   CoverageTypes = "[\"Property\",\"Equipment\"]" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000004"), PolicyNumber = "POL-2025-015", ClientName = "Sarah Johnson",          EffectiveDate = new DateOnly(2025, 1, 1),  ExpirationDate = new DateOnly(2027, 12, 31), Status = PolicyStatus.Active,    CoverageTypes = "[\"Auto\",\"Liability\"]" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000005"), PolicyNumber = "POL-2024-078", ClientName = "Riverside Medical Group", EffectiveDate = new DateOnly(2024, 7, 1),  ExpirationDate = new DateOnly(2026, 6, 30),  Status = PolicyStatus.Active,    CoverageTypes = "[\"Property\",\"Liability\",\"Equipment\"]" },
            new { Id = new Guid("20000000-0000-0000-0000-000000000006"), PolicyNumber = "POL-2022-034", ClientName = "TechStart Inc",          EffectiveDate = new DateOnly(2022, 4, 1),  ExpirationDate = new DateOnly(2024, 3, 31),  Status = PolicyStatus.Cancelled, CoverageTypes = "[\"Property\"]" }
        );
    }
}

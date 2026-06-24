using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public sealed class ClaimConfiguration : BaseEntityConfiguration<Claim>
{
    public override void Configure(EntityTypeBuilder<Claim> builder)
    {
        base.Configure(builder);

        builder.ToTable("Claims");

        builder.Property(e => e.OrganisationId)
            .IsRequired();

        builder.Property(e => e.ClaimNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(e => e.ClaimNumber)
            .IsUnique();

        builder.HasIndex(e => e.OrganisationId);

        builder.Property(e => e.PolicyId)
            .IsRequired(false);

        builder.Property(e => e.PolicyNumber)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(e => e.ClientName)
            .HasMaxLength(255)
            .IsRequired(false);

        // Enum → string конвенція застосовується глобально в ConfigureConventions
        builder.Property(e => e.Status)
            .IsRequired();

        builder.Property(e => e.Severity)
            .IsRequired();

        builder.Property(e => e.ReportedDate)
            .IsRequired();

        builder.Property(e => e.AssignedHandlerId)
            .IsRequired(false);

        builder.Property(e => e.ClosedAt)
            .IsRequired(false);

        builder.Property(e => e.ClosureReason)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(e => e.Notes)
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        // Optimistic concurrency
        builder.Property(e => e.RowVer)
            .IsRowVersion();

        // Relationships
        builder.HasOne(e => e.LossEvent)
            .WithOne(e => e.Claim)
            .HasForeignKey<LossEvent>(e => e.ClaimId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Parties)
            .WithOne(e => e.Claim)
            .HasForeignKey(e => e.ClaimId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.RiskObjects)
            .WithOne(e => e.Claim)
            .HasForeignKey(e => e.ClaimId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Reserves)
            .WithOne(e => e.Claim)
            .HasForeignKey(e => e.ClaimId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Documents)
            .WithOne(e => e.Claim)
            .HasForeignKey(e => e.ClaimId)
            .OnDelete(DeleteBehavior.Cascade);

        // Policy FK (nullable — simulated policy table)
        builder.HasOne(e => e.Policy)
            .WithMany()
            .HasForeignKey(e => e.PolicyId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // Private collection backing fields
        builder.Navigation(e => e.Parties)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(e => e.RiskObjects)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(e => e.Reserves)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Navigation(e => e.Documents)
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}

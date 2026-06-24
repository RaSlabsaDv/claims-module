using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public sealed class ClaimSequenceConfiguration : IEntityTypeConfiguration<ClaimSequence>
{
    public void Configure(EntityTypeBuilder<ClaimSequence> builder)
    {
        builder.ToTable("ClaimSequences");

        // Composite PK — OrganisationId + Year
        builder.HasKey(e => new { e.OrganisationId, e.Year });

        builder.Property(e => e.OrganisationId)
            .IsRequired();

        builder.Property(e => e.Year)
            .IsRequired();

        builder.Property(e => e.LastSequence)
            .IsRequired()
            .HasDefaultValue(0L);
    }
}

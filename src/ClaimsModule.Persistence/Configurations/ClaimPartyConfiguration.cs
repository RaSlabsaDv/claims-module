using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public sealed class ClaimPartyConfiguration : BaseEntityConfiguration<ClaimParty>
{
    public override void Configure(EntityTypeBuilder<ClaimParty> builder)
    {
        base.Configure(builder);

        builder.ToTable("ClaimParties");

        builder.Property(e => e.ClaimId)
            .IsRequired();

        builder.Property(e => e.PartyRole)
            .IsRequired();

        builder.Property(e => e.PartyType)
            .IsRequired();

        builder.Property(e => e.FirstName)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(e => e.LastName)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(e => e.CompanyName)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(e => e.Email)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(e => e.Phone)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(e => e.Notes)
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        builder.Property(e => e.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
    }
}

using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public sealed class ClaimRiskObjectConfiguration : BaseEntityConfiguration<ClaimRiskObject>
{
    public override void Configure(EntityTypeBuilder<ClaimRiskObject> builder)
    {
        base.Configure(builder);

        builder.ToTable("ClaimRiskObjects");

        builder.Property(e => e.ClaimId)
            .IsRequired();

        builder.Property(e => e.AssetType)
            .IsRequired();

        builder.Property(e => e.AssetDescription)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.DamageDescription)
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        builder.Property(e => e.IsPrimary)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.AssetReference)
            .HasMaxLength(255)
            .IsRequired(false);
    }
}

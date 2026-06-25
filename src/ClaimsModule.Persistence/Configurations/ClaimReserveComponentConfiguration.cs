using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public sealed class ClaimReserveComponentConfiguration : BaseEntityConfiguration<ClaimReserveComponent>
{
    public override void Configure(EntityTypeBuilder<ClaimReserveComponent> builder)
    {
        base.Configure(builder);

        builder.ToTable("ClaimReserveComponents");

        builder.Property(e => e.ClaimId)
            .IsRequired();

        builder.Property(e => e.ComponentType)
            .IsRequired();

        builder.Property(e => e.CurrentAmount)
            .IsRequired()
            .HasPrecision(19, 4)
            .HasDefaultValue(0m);

        builder.Property(e => e.Status)
            .IsRequired();

        builder.Property(e => e.Notes)
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        // Optimistic concurrency — другий aggregate root
        builder.Property(e => e.RowVer)
            .IsRowVersion();

        // Один тип компоненту на claim (Indemnity, Expense тощо — по одному)
        builder.HasIndex(e => new { e.ClaimId, e.ComponentType })
            .IsUnique();

        builder.HasMany(e => e.Transactions)
            .WithOne(e => e.ReserveComponent)
            .HasForeignKey(e => e.ReserveComponentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(e => e.Transactions)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.Property(e => e.ManagerOverrideFlag)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.ManagerOverrideAt)
            .IsRequired(false);

        builder.Property(e => e.ManagerOverrideByUserId)
            .IsRequired(false);
    }
}

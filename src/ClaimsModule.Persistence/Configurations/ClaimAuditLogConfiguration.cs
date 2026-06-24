using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public sealed class ClaimAuditLogConfiguration : IEntityTypeConfiguration<ClaimAuditLog>
{
    public void Configure(EntityTypeBuilder<ClaimAuditLog> builder)
    {
        builder.ToTable("ClaimAuditLogs");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(e => e.ClaimId)
            .IsRequired();

        builder.Property(e => e.EventType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(e => e.OldValue)
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        builder.Property(e => e.NewValue)
            .HasColumnType("nvarchar(max)")
            .IsRequired(false);

        builder.Property(e => e.RelatedEntityId)
            .IsRequired(false);

        builder.Property(e => e.RelatedEntityType)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(e => e.CorrelationId)
            .IsRequired(false);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedByUserId)
            .IsRequired(false);

        // Індекси для частих запитів
        builder.HasIndex(e => e.ClaimId);
        builder.HasIndex(e => new { e.ClaimId, e.EventType });
        builder.HasIndex(e => e.CreatedAt);

        // Immutable — без query filter, без soft delete
    }
}

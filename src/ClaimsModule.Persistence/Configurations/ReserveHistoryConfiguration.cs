using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public sealed class ReserveHistoryConfiguration : IEntityTypeConfiguration<ReserveHistory>
{
    public void Configure(EntityTypeBuilder<ReserveHistory> builder)
    {
        builder.ToTable("ReserveHistories");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(e => e.ReserveComponentId)
            .IsRequired();

        builder.Property(e => e.ClaimId)
            .IsRequired();

        builder.Property(e => e.TransactionType)
            .IsRequired();

        builder.Property(e => e.Amount)
            .IsRequired()
            .HasPrecision(19, 4);

        builder.Property(e => e.PreviousBalance)
            .IsRequired()
            .HasPrecision(19, 4);

        builder.Property(e => e.NewBalance)
            .IsRequired()
            .HasPrecision(19, 4);

        builder.Property(e => e.ChangeReason)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.ApprovalStatus)
            .IsRequired();

        builder.Property(e => e.ApprovedByUserId)
            .IsRequired(false);

        builder.Property(e => e.ApprovedAt)
            .IsRequired(false);

        builder.Property(e => e.RejectedByUserId)
            .IsRequired(false);

        builder.Property(e => e.RejectedAt)
            .IsRequired(false);

        builder.Property(e => e.RejectionReason)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(e => e.PostingStatus)
            .IsRequired();

        builder.Property(e => e.PostingJobId)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(e => e.IdempotencyKey)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(e => e.IdempotencyKey)
            .IsUnique();

        builder.Property(e => e.ChangeSequence)
            .IsRequired();

        builder.Property(e => e.SubmittedByUserId)
            .IsRequired(false);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // Append-only — без query filter (немає IsDeleted)

        builder.HasIndex(e => new { e.ReserveComponentId, e.ChangeSequence })
            .IsUnique();
    }
}

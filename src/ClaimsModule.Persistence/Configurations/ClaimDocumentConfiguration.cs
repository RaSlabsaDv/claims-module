using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public sealed class ClaimDocumentConfiguration : BaseEntityConfiguration<ClaimDocument>
{
    public override void Configure(EntityTypeBuilder<ClaimDocument> builder)
    {
        base.Configure(builder);

        builder.ToTable("ClaimDocuments");

        builder.Property(e => e.ClaimId)
            .IsRequired();

        builder.Property(e => e.DocumentType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.DocumentName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(e => e.BlobPath)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.ContentType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.FileSizeBytes)
            .IsRequired();

        builder.Property(e => e.UploadedAt)
            .IsRequired();

        builder.Property(e => e.UploadedByUserId)
            .IsRequired(false);

        builder.Property(e => e.Notes)
            .HasMaxLength(500)
            .IsRequired(false);
    }
}

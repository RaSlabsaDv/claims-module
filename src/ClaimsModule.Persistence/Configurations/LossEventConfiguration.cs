using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public sealed class LossEventConfiguration : BaseEntityConfiguration<LossEvent>
{
    public override void Configure(EntityTypeBuilder<LossEvent> builder)
    {
        base.Configure(builder);

        builder.ToTable("LossEvents");

        builder.Property(e => e.ClaimId)
            .IsRequired();

        builder.Property(e => e.LossDate)
            .IsRequired();

        builder.Property(e => e.LossDescription)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.Property(e => e.LossLocation)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(e => e.CauseOfLossCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.EstimatedLossAmount)
            .HasPrecision(19, 4)
            .IsRequired(false);

        builder.Property(e => e.ReportDate)
            .IsRequired();

        builder.Property(e => e.PoliceReportNumber)
            .HasMaxLength(100)
            .IsRequired(false);

        // FK до CauseOfLossCodes по Code (не по Id)
        builder.HasOne(e => e.CauseOfLossCodeRef)
            .WithMany()
            .HasForeignKey(e => e.CauseOfLossCode)
            .HasPrincipalKey(e => e.Code)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

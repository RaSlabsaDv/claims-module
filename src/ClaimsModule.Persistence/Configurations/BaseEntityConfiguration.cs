using ClaimsModule.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClaimsModule.Persistence.Configurations;

public abstract class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // PK — NEWSEQUENTIALID() уникає фрагментації індексів
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        // Audit columns
        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt)
            .IsRequired(false);

        builder.Property(e => e.UserCreated)
            .IsRequired(false);

        builder.Property(e => e.UserModified)
            .IsRequired(false);

        // Soft delete
        builder.Property(e => e.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.DeletedAt)
            .IsRequired(false);

        // Global query filter — автоматично фільтрує soft-deleted записи
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

using ClaimsModule.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClaimsModule.Persistence;

public sealed class ClaimsDbContext : DbContext
{
    // ---------------------------------------------------------------------------
    // DbSets
    // ---------------------------------------------------------------------------
    public DbSet<Claim> Claims => Set<Claim>();
    public DbSet<LossEvent> LossEvents => Set<LossEvent>();
    public DbSet<ClaimParty> ClaimParties => Set<ClaimParty>();
    public DbSet<ClaimRiskObject> ClaimRiskObjects => Set<ClaimRiskObject>();
    public DbSet<ClaimReserveComponent> ClaimReserveComponents => Set<ClaimReserveComponent>();
    public DbSet<ReserveHistory> ReserveHistories => Set<ReserveHistory>();
    public DbSet<ClaimDocument> ClaimDocuments => Set<ClaimDocument>();
    public DbSet<ClaimAuditLog> ClaimAuditLogs => Set<ClaimAuditLog>();
    public DbSet<CauseOfLossCode> CauseOfLossCodes => Set<CauseOfLossCode>();
    public DbSet<Policy> Policies => Set<Policy>();
    public DbSet<ClaimSequence> ClaimSequences => Set<ClaimSequence>();

    public ClaimsDbContext(DbContextOptions<ClaimsDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Підхоплює всі IEntityTypeConfiguration<T> з поточного assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClaimsDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Глобальна конвенція: всі enum properties зберігаються як string (NVARCHAR)
        // Це читабельніше в БД і не ламається при зміні порядку enum values
        configurationBuilder
            .Properties<Enum>()
            .HaveConversion<string>()
            .HaveMaxLength(50);

        // Глобальна конвенція: DateTimeOffset → DATETIMEOFFSET(7)
        configurationBuilder
            .Properties<DateTimeOffset>()
            .HaveColumnType("datetimeoffset(7)");

        configurationBuilder
            .Properties<DateTimeOffset?>()
            .HaveColumnType("datetimeoffset(7)");
    }
}

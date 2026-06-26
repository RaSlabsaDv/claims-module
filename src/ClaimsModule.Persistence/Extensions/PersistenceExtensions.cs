using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Persistence.Repositories;
using ClaimsModule.Persistence.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClaimsModule.Persistence;

public static class PersistenceExtensions
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ClaimsDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(ClaimsDbContext).Assembly.FullName)));

        // UoW
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Repositories
        services.AddScoped<IClaimRepository, ClaimRepository>();
        services.AddScoped<IReserveRepository, ReserveRepository>();
        services.AddScoped<IClaimPartyRepository, ClaimPartyRepository>();
        services.AddScoped<IClaimRiskObjectRepository, ClaimRiskObjectRepository>();
        services.AddScoped<IClaimDocumentRepository, ClaimDocumentRepository>();
        services.AddScoped<IClaimAuditLogRepository, ClaimAuditLogRepository>();
        services.AddScoped<IClaimAuditLogWriter, ClaimAuditLogWriter>();
        services.AddScoped<IPolicyRepository, PolicyRepository>();
        services.AddScoped<ICauseOfLossCodeRepository, CauseOfLossCodeRepository>();

        // Services
        services.AddScoped<IClaimNumberGenerator, ClaimNumberGenerator>();

        return services;
    }
}
using Azure.Storage.Blobs;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Application.Services;
using ClaimsModule.Infrastructure.Jobs;
using ClaimsModule.Infrastructure.Services;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ClaimsModule.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Storage
        var storageProvider = configuration["Storage:Provider"];

        if (storageProvider == "AzureBlob")
        {
            var connectionString = configuration["Storage:AzureBlob:ConnectionString"]!;
            var containerName = configuration["Storage:AzureBlob:ContainerName"]!;

            services.AddSingleton(_ => new BlobServiceClient(connectionString));
            services.AddScoped<IStorageService>(sp =>
                new AzureBlobStorageService(
                    sp.GetRequiredService<BlobServiceClient>(),
                    containerName));
        }
        else
        {
            var basePath = configuration["Storage:LocalFileSystem:BasePath"] ?? "/uploads";
            var baseUrl = configuration["Storage:LocalFileSystem:BaseUrl"] ?? "http://localhost:5001";

            services.AddScoped<IStorageService>(_ =>
                new LocalFileSystemStorageService(basePath, baseUrl));
        }

        // Hangfire
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"),
                new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    DisableGlobalLocks = true
                }));

        services.AddHangfireServer();

        // Job Schedulers
        services.AddScoped<IGlPostingJobScheduler, GlPostingJobScheduler>();

        // Jobs
        services.AddScoped<SlaMonitoringJob>();

        return services;
    }

    public static void RegisterRecurringJobs()
    {
        RecurringJob.AddOrUpdate<SlaMonitoringJob>(
            "sla-monitoring",
            job => job.ExecuteAsync(),
            "*/15 * * * *");
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ClaimsModule.Persistence;

public sealed class ClaimsDbContextFactory : IDesignTimeDbContextFactory<ClaimsDbContext>
{
    public ClaimsDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../ClaimsModule.API"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ClaimsDbContext>();
        optionsBuilder.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"));

        return new ClaimsDbContext(optionsBuilder.Options);
    }
}
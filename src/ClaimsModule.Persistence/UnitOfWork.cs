using ClaimsModule.Application.Common.Interfaces;

namespace ClaimsModule.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ClaimsDbContext _context;

    public UnitOfWork(ClaimsDbContext context)
        => _context = context;

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
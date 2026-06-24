namespace ClaimsModule.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IClaimRepository Claims { get; }
    IReserveRepository Reserves { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

using ClaimsModule.Domain.Entities;

namespace ClaimsModule.Application.Common.Interfaces;

public interface IReserveRepository
{
    Task<ClaimReserveComponent?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ClaimReserveComponent?> GetByIdWithTransactionsAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ClaimReserveComponent>> ListByClaimAsync(Guid claimId, CancellationToken ct = default);

    Task<ReserveHistory?> GetTransactionByIdAsync(Guid transactionId, CancellationToken ct = default);

    Task AddAsync(ClaimReserveComponent reserve, CancellationToken ct = default);

    // Aggregate total для перевірки $10M ліміту (BR-R-05)
    Task<decimal> GetAggregateTotalByClaimAsync(Guid claimId, CancellationToken ct = default);

    Task<int> GetNextChangeSequenceAsync(Guid reserveId, CancellationToken ct = default);
}

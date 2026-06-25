using ClaimsModule.Domain.Common;
using ClaimsModule.Domain.Constants;
using ClaimsModule.Domain.Enums;
using ClaimsModule.Domain.Exceptions;

namespace ClaimsModule.Domain.Entities;

public sealed class ClaimReserveComponent : BaseEntity
{
    public Guid ClaimId { get; private set; }
    public Claim Claim { get; private set; } = default!;

    public ReserveComponentType ComponentType { get; private set; }
    public decimal CurrentAmount { get; private set; }
    public ReserveComponentStatus Status { get; private set; }
    public string? Notes { get; private set; }

    // Optimistic concurrency — другий aggregate root
    public byte[] RowVer { get; private set; } = default!;

    private readonly List<ReserveHistory> _transactions = [];
    public IReadOnlyList<ReserveHistory> Transactions => _transactions.AsReadOnly();

    // EF Core
    private ClaimReserveComponent() { }

    public static ClaimReserveComponent Create(
        Guid claimId,
        ReserveComponentType componentType,
        Guid createdByUserId,
        string? notes = null)
    {
        var reserve = new ClaimReserveComponent
        {
            ClaimId = claimId,
            ComponentType = componentType,
            CurrentAmount = 0,
            Status = ReserveComponentStatus.Active,
            Notes = notes
        };

        reserve.SetCreated(createdByUserId);

        return reserve;
    }

    // ---------------------------------------------------------------------------
    // Транзакції
    // ---------------------------------------------------------------------------
    public ReserveHistory AddTransaction(
        decimal amount,
        ReserveTransactionType transactionType,
        string changeReason,
        Guid submittedByUserId,
        int changeSequence)
    {
        if (Status == ReserveComponentStatus.Closed)
            throw new DomainException("Cannot add a transaction to a closed reserve component.");

        // SubrogationRecoverable може бути від'ємним, решта — тільки > 0
        if (ComponentType != ReserveComponentType.SubrogationRecoverable && amount <= 0)
            throw new DomainException("Reserve amount must be greater than zero.");

        var previousBalance = CurrentAmount;
        var newBalance = previousBalance + amount;

        var transaction = ReserveHistory.Create(
            reserveComponentId: Id,
            claimId: ClaimId,
            transactionType: transactionType,
            amount: amount,
            previousBalance: previousBalance,
            newBalance: newBalance,
            changeReason: changeReason,
            submittedByUserId: submittedByUserId,
            changeSequence: changeSequence);

        _transactions.Add(transaction);

        return transaction;
    }

    public void ApplyApprovedTransaction(Guid transactionId, decimal newBalance)
    {
        CurrentAmount = newBalance;
        SetUpdated(Guid.Empty); // system update
    }

    public void Close(Guid userId)
    {
        Status = ReserveComponentStatus.Closed;
        SetUpdated(userId);
    }

    // ---------------------------------------------------------------------------
    // Authority Level (BR-R-01)
    // ---------------------------------------------------------------------------
    public static ApprovalLevel DetermineRequiredApprovalLevel(decimal amount) =>
        amount switch
        {
            <= ReserveThresholds.AutoApprovalLimit  => ApprovalLevel.AutoApproved,
            <= ReserveThresholds.SupervisorLimit    => ApprovalLevel.Supervisor,
            <= ReserveThresholds.ManagerLimit       => ApprovalLevel.Manager,
            _                                       => ApprovalLevel.Manager
        };
}

using ClaimsModule.Domain.Enums;
using ClaimsModule.Domain.Exceptions;

namespace ClaimsModule.Domain.Entities;

// Append-only — не наслідує BaseEntity (немає UpdatedAt, IsDeleted)
public sealed class ReserveHistory
{
    public Guid Id { get; private set; }
    public Guid ReserveComponentId { get; private set; }
    public ClaimReserveComponent ReserveComponent { get; private set; } = default!;

    // Денормалізовано для зручності запитів
    public Guid ClaimId { get; private set; }

    public ReserveTransactionType TransactionType { get; private set; }
    public decimal Amount { get; private set; }
    public decimal PreviousBalance { get; private set; }
    public decimal NewBalance { get; private set; }
    public string ChangeReason { get; private set; } = default!;

    // Approval
    public ReserveApprovalStatus ApprovalStatus { get; private set; }
    public Guid? ApprovedByUserId { get; private set; }
    public DateTimeOffset? ApprovedAt { get; private set; }
    public Guid? RejectedByUserId { get; private set; }
    public DateTimeOffset? RejectedAt { get; private set; }
    public string? RejectionReason { get; private set; }

    // GL Posting
    public GlPostingStatus PostingStatus { get; private set; }
    public string? PostingJobId { get; private set; }
    public string IdempotencyKey { get; private set; } = default!;
    public int ChangeSequence { get; private set; }

    public Guid? SubmittedByUserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    // EF Core
    private ReserveHistory() { }

    internal static ReserveHistory Create(
        Guid reserveComponentId,
        Guid claimId,
        ReserveTransactionType transactionType,
        decimal amount,
        decimal previousBalance,
        decimal newBalance,
        string changeReason,
        Guid submittedByUserId,
        int changeSequence)
    {
        return new ReserveHistory
        {
            Id = Guid.NewGuid(),
            ReserveComponentId = reserveComponentId,
            ClaimId = claimId,
            TransactionType = transactionType,
            Amount = amount,
            PreviousBalance = previousBalance,
            NewBalance = newBalance,
            ChangeReason = changeReason,
            SubmittedByUserId = submittedByUserId,
            ChangeSequence = changeSequence,
            ApprovalStatus = ReserveApprovalStatus.PendingApproval,
            PostingStatus = GlPostingStatus.Pending,
            IdempotencyKey = $"Reserve:{reserveComponentId}:Change:{changeSequence}",
            CreatedAt = DateTimeOffset.UtcNow
        };
    }

    // ---------------------------------------------------------------------------
    // Approval / Rejection / Cancellation
    // ---------------------------------------------------------------------------
    public void Approve(Guid approvedByUserId)
    {
        if (ApprovalStatus != ReserveApprovalStatus.PendingApproval)
            throw new DomainException($"Cannot approve a transaction with status {ApprovalStatus}.");

        ApprovalStatus = ReserveApprovalStatus.Approved;
        ApprovedByUserId = approvedByUserId;
        ApprovedAt = DateTimeOffset.UtcNow;
    }

    public void AutoApprove()
    {
        ApprovalStatus = ReserveApprovalStatus.AutoApproved;
        ApprovedAt = DateTimeOffset.UtcNow;
    }

    public void Reject(Guid rejectedByUserId, string rejectionReason)
    {
        if (ApprovalStatus != ReserveApprovalStatus.PendingApproval)
            throw new DomainException($"Cannot reject a transaction with status {ApprovalStatus}.");

        ApprovalStatus = ReserveApprovalStatus.Rejected;
        RejectedByUserId = rejectedByUserId;
        RejectedAt = DateTimeOffset.UtcNow;
        RejectionReason = rejectionReason;
    }

    public void Cancel()
    {
        if (ApprovalStatus != ReserveApprovalStatus.PendingApproval)
            throw new DomainException($"Cannot cancel a transaction with status {ApprovalStatus}.");

        ApprovalStatus = ReserveApprovalStatus.Cancelled;
    }

    // ---------------------------------------------------------------------------
    // GL Posting
    // ---------------------------------------------------------------------------
    public void MarkPosted(string jobId)
    {
        PostingStatus = GlPostingStatus.Posted;
        PostingJobId = jobId;
    }

    public void MarkPostingFailed()
        => PostingStatus = GlPostingStatus.Failed;
}

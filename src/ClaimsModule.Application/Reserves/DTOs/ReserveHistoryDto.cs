using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Application.Reserves.Dtos;

public sealed record ReserveHistoryDto(
    Guid Id,
    ReserveTransactionType TransactionType,
    decimal Amount,
    decimal PreviousBalance,
    decimal NewBalance,
    string ChangeReason,
    ReserveApprovalStatus ApprovalStatus,
    Guid? ApprovedByUserId,
    DateTimeOffset? ApprovedAt,
    Guid? RejectedByUserId,
    DateTimeOffset? RejectedAt,
    string? RejectionReason,
    GlPostingStatus PostingStatus,
    int ChangeSequence,
    Guid? SubmittedByUserId,
    DateTimeOffset CreatedAt
);
using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Application.Reference.Queries.GetClaimStatuses;

public sealed record ClaimStatusDto(
    ClaimStatus Status,
    IReadOnlyList<ClaimStatus> ValidTransitions
);
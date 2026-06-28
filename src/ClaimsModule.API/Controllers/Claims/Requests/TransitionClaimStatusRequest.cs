using ClaimsModule.Domain.Enums;

namespace ClaimsModule.API.Controllers.Claims;

public sealed record TransitionClaimStatusRequest(
    string RowVersion,
    ClaimStatus TargetStatus,
    string? Reason);
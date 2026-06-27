using ClaimsModule.Domain.Enums;

namespace ClaimsModule.API.Controllers.Claims;

public sealed record TransitionClaimStatusRequest(
    byte[] RowVersion,
    ClaimStatus TargetStatus,
    string? Reason);
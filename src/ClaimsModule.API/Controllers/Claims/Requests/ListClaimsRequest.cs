using ClaimsModule.Domain.Enums;

namespace ClaimsModule.API.Controllers.Claims;

public sealed record ListClaimsRequest(
    ClaimStatus? Status,
    DateTimeOffset? DateFrom,
    DateTimeOffset? DateTo,
    Guid? AssignedHandlerId,
    string? CauseOfLossCode,
    Guid? PolicyId,
    string? Search,
    int Page = 1,
    int PageSize = 20);
namespace ClaimsModule.Application.Claims.Queries.ListClaims;

public sealed record ListClaimsResult(
    IReadOnlyList<ClaimListItemDto> Items,
    int TotalCount);
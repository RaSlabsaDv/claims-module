namespace ClaimsModule.Application.Claims.Queries.ListClaims;

public sealed record ClaimListItemDto(
    Guid Id,
    string ClaimNumber,
    string? PolicyNumber,
    string? ClientName,
    string Status,
    string Severity,
    DateTimeOffset ReportedDate,
    string? CauseOfLossCode,
    Guid? AssignedHandlerId,
    DateTimeOffset? UpdatedAt
);
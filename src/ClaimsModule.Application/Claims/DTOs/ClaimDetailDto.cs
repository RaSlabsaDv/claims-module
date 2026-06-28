namespace ClaimsModule.Application.Claims.Dtos;

public sealed record ClaimDetailDto(
    Guid Id,
    string ClaimNumber,
    Guid OrganisationId,
    Guid? PolicyId,
    string? PolicyNumber,
    string? ClientName,
    string Status,
    string Severity,
    DateTimeOffset ReportedDate,
    DateTimeOffset? ClosedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,
    Guid? AssignedHandlerId,
    string? ClosureReason,
    string? Notes,
    byte[] RowVer,
    LossEventDto? LossEvent,
    IReadOnlyList<ClaimPartyDto> Parties,
    IReadOnlyList<ClaimRiskObjectDto> RiskObjects
);
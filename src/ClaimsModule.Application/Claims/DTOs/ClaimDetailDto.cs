namespace ClaimsModule.Application.Claims.Dtos;

public sealed record ClaimDetailDto(
    Guid Id,
    string ClaimNumber,
    Guid OrganisationId,

    // Policy
    Guid? PolicyId,
    string? PolicyNumber,
    string? ClientName,

    // Status
    string Status,
    string Severity,

    // Dates
    DateTimeOffset ReportedDate,
    DateTimeOffset? ClosedAt,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt,

    // Assignment
    Guid? AssignedHandlerId,
    string? ClosureReason,
    string? Notes,

    // Related
    LossEventDto? LossEvent,
    IReadOnlyList<ClaimPartyDto> Parties,
    IReadOnlyList<ClaimRiskObjectDto> RiskObjects,
    IReadOnlyList<ClaimDocumentDto> Documents
);
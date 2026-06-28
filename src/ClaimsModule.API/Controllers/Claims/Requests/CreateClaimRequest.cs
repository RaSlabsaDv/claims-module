using ClaimsModule.Domain.Enums;

namespace ClaimsModule.API.Controllers.Claims;

public sealed record CreateClaimRequest(
    Guid? PolicyId,
    string? PolicyNumber,
    string? ClientName,
    ClaimSeverity Severity,
    Guid? AssignedHandlerId,
    string? Notes,
    DateTimeOffset LossDate,
    string LossDescription,
    string? LossLocation,
    string CauseOfLossCode,
    decimal? EstimatedLossAmount,
    string? PoliceReportNumber);
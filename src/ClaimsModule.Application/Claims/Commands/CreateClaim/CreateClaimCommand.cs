using ClaimsModule.Domain.Enums;
using MediatR;

namespace ClaimsModule.Application.Claims.Commands.CreateClaim;

public sealed record CreateClaimCommand(
    Guid OrganisationId,
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
    string? PoliceReportNumber) : IRequest<CreateClaimResult>;

public sealed record CreateClaimResult(
    Guid ClaimId,
    string ClaimNumber);
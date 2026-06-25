namespace ClaimsModule.Application.Claims.Dtos;

public sealed record LossEventDto(
    Guid Id,
    DateTimeOffset LossDate,
    string LossDescription,
    string? LossLocation,
    string CauseOfLossCode,
    decimal? EstimatedLossAmount,
    DateTimeOffset ReportDate,
    string? PoliceReportNumber
);
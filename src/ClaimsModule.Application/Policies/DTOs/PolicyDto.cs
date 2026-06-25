using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Application.Policies.Dtos;

public sealed record PolicyDto(
    Guid Id,
    string PolicyNumber,
    string ClientName,
    DateOnly EffectiveDate,
    DateOnly ExpirationDate,
    PolicyStatus Status,
    IReadOnlyList<string> CoverageTypes
);
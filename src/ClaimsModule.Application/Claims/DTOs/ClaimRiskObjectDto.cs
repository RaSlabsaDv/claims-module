namespace ClaimsModule.Application.Claims.Dtos;

public sealed record ClaimRiskObjectDto(
    Guid Id,
    string AssetType,
    string AssetDescription,
    string? DamageDescription,
    bool IsPrimary,
    string? AssetReference
);
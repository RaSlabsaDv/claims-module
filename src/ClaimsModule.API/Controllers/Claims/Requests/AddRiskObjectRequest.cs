using ClaimsModule.Domain.Enums;

namespace ClaimsModule.API.Controllers.Claims;

public sealed record AddRiskObjectRequest(
    string RowVersion,
    AssetType AssetType,
    string AssetDescription,
    string? DamageDescription,
    bool IsPrimary,
    string? AssetReference);
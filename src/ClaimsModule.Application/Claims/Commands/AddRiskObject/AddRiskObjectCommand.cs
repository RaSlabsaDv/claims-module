using ClaimsModule.Domain.Enums;
using MediatR;

namespace ClaimsModule.Application.Claims.Commands.AddRiskObject;

public sealed record AddRiskObjectCommand(
    Guid ClaimId,
    AssetType AssetType,
    string AssetDescription,
    string? DamageDescription,
    bool IsPrimary,
    string? AssetReference) : IRequest<Guid>;
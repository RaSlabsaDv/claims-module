using ClaimsModule.Domain.Common;
using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Domain.Entities;

public sealed class ClaimRiskObject : BaseEntity
{
    public Guid ClaimId { get; private set; }
    public Claim Claim { get; private set; } = default!;

    public AssetType AssetType { get; private set; }
    public string AssetDescription { get; private set; } = default!;
    public string? DamageDescription { get; private set; }
    public bool IsPrimary { get; private set; }
    public string? AssetReference { get; private set; }

    // EF Core
    private ClaimRiskObject() { }

    public static ClaimRiskObject Create(
        Guid claimId,
        AssetType assetType,
        string assetDescription,
        Guid createdByUserId,
        string? damageDescription = null,
        bool isPrimary = false,
        string? assetReference = null)
    {
        var riskObject = new ClaimRiskObject
        {
            ClaimId = claimId,
            AssetType = assetType,
            AssetDescription = assetDescription,
            DamageDescription = damageDescription,
            IsPrimary = isPrimary,
            AssetReference = assetReference
        };

        riskObject.SetCreated(createdByUserId);

        return riskObject;
    }

    public void Update(
        AssetType assetType,
        string assetDescription,
        Guid updatedByUserId,
        string? damageDescription = null,
        bool isPrimary = false,
        string? assetReference = null)
    {
        AssetType = assetType;
        AssetDescription = assetDescription;
        DamageDescription = damageDescription;
        IsPrimary = isPrimary;
        AssetReference = assetReference;

        SetUpdated(updatedByUserId);
    }
}

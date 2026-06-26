using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Constants;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Exceptions;
using MediatR;

namespace ClaimsModule.Application.Claims.Commands.AddRiskObject;

public sealed class AddRiskObjectCommandHandler(
    IClaimRepository claimRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IAuditLogService auditLog)
    : IRequestHandler<AddRiskObjectCommand, Guid>
{
    public async Task<Guid> Handle(AddRiskObjectCommand request, CancellationToken ct)
    {
        var claim = await claimRepository.GetByIdAsync(request.ClaimId, ct)
            ?? throw new NotFoundException(nameof(Claim), request.ClaimId);

        claimRepository.SetOriginalRowVersion(claim, request.RowVersion);

        var riskObject = ClaimRiskObject.Create(
            claimId: request.ClaimId,
            assetType: request.AssetType,
            assetDescription: request.AssetDescription,
            createdByUserId: currentUser.UserId,
            damageDescription: request.DamageDescription,
            isPrimary: request.IsPrimary,
            assetReference: request.AssetReference);

        claim.AddRiskObject(riskObject);

        await unitOfWork.SaveChangesAsync(ct);

        await auditLog.LogAsync(
            claimId: claim.Id,
            eventType: AuditEventTypes.RiskObjectAdded,
            description: $"Risk object {request.AssetType} — {request.AssetDescription} added to claim.",
            relatedEntityId: riskObject.Id,
            relatedEntityType: nameof(ClaimRiskObject),
            newValue: new
            {
                request.AssetType,
                request.AssetDescription,
                request.DamageDescription,
                request.IsPrimary,
                request.AssetReference
            });

        return riskObject.Id;
    }
}
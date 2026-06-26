using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Constants;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Exceptions;
using MediatR;

namespace ClaimsModule.Application.Claims.Commands.RemoveParty;

public sealed class RemovePartyCommandHandler(
    IClaimRepository claimRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IAuditLogService auditLog)
    : IRequestHandler<RemovePartyCommand, Unit>
{
    public async Task<Unit> Handle(RemovePartyCommand request, CancellationToken ct)
    {
        var claim = await claimRepository.GetByIdWithDetailsAsync(request.ClaimId, ct)
            ?? throw new NotFoundException(nameof(Claim), request.ClaimId);

        if (!claim.RowVer.SequenceEqual(request.RowVersion))
            throw new ConcurrencyException(nameof(Claim), request.ClaimId);

        claim.RemoveParty(request.PartyId, currentUser.UserId);

        await unitOfWork.SaveChangesAsync(ct);

        await auditLog.LogAsync(
            claimId: claim.Id,
            eventType: AuditEventTypes.PartyRemoved,
            description: "Party removed from claim.",
            relatedEntityId: request.PartyId,
            relatedEntityType: nameof(ClaimParty));

        return Unit.Value;
    }
}
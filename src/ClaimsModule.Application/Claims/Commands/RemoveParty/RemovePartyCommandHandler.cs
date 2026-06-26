using ClaimsModule.Application.Common.Exceptions;
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
        var userId = currentUser.UserId ?? throw new UnauthorizedException();

        var claim = await claimRepository.GetByIdWithDetailsAsync(request.ClaimId, ct)
            ?? throw new NotFoundException(nameof(Claim), request.ClaimId);

        claimRepository.SetOriginalRowVersion(claim, request.RowVersion);

        claim.RemoveParty(request.PartyId, userId);

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
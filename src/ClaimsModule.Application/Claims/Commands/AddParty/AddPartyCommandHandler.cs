using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Domain.Constants;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Exceptions;
using MediatR;

namespace ClaimsModule.Application.Claims.Commands.AddParty;

public sealed class AddPartyCommandHandler(
    IClaimRepository claimRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUser,
    IAuditLogService auditLog)
    : IRequestHandler<AddPartyCommand, Guid>
{
    public async Task<Guid> Handle(AddPartyCommand request, CancellationToken ct)
    {
        var claim = await claimRepository.GetByIdAsync(request.ClaimId, ct)
            ?? throw new NotFoundException(nameof(Claim), request.ClaimId);

        if (!claim.RowVer.SequenceEqual(request.RowVersion))
            throw new ConcurrencyException(nameof(Claim), request.ClaimId);

        var party = ClaimParty.Create(
            claimId: request.ClaimId,
            partyRole: request.PartyRole,
            partyType: request.PartyType,
            createdByUserId: currentUser.UserId,
            firstName: request.FirstName,
            lastName: request.LastName,
            companyName: request.CompanyName,
            email: request.Email,
            phone: request.Phone,
            notes: request.Notes);

        claim.AddParty(party);

        await unitOfWork.SaveChangesAsync(ct);

        await auditLog.LogAsync(
            claimId: claim.Id,
            eventType: AuditEventTypes.PartyAdded,
            description: $"Party {request.PartyRole} added to claim.",
            relatedEntityId: party.Id,
            relatedEntityType: nameof(ClaimParty),
            newValue: new { request.PartyRole, request.PartyType });

        return party.Id;
    }
}
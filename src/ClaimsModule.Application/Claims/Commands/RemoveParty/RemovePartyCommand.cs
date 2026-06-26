using MediatR;

namespace ClaimsModule.Application.Claims.Commands.RemoveParty;

public sealed record RemovePartyCommand(
    Guid ClaimId,
    Guid PartyId,
    byte[] RowVersion) : IRequest<Unit>;
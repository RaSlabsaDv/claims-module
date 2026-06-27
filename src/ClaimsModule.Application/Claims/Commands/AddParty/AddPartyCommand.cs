using ClaimsModule.Domain.Enums;
using MediatR;

namespace ClaimsModule.Application.Claims.Commands.AddParty;

public sealed record AddPartyCommand(
    Guid ClaimId,
    PartyRole PartyRole,
    PartyType PartyType,
    string? FirstName,
    string? LastName,
    string? CompanyName,
    string? Email,
    string? Phone,
    string? Notes,
    byte[] RowVersion) : IRequest<Guid>;
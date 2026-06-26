using ClaimsModule.Domain.Enums;
using MediatR;

public sealed record AddPartyCommand(
    Guid ClaimId,
    byte[] RowVersion,
    PartyRole PartyRole,
    PartyType PartyType,
    string? FirstName,
    string? LastName,
    string? CompanyName,
    string? Email,
    string? Phone,
    string? Notes) : IRequest<Guid>;
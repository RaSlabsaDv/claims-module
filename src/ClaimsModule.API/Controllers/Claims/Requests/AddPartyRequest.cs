using ClaimsModule.Domain.Enums;

namespace ClaimsModule.API.Controllers.Claims;

public sealed record AddPartyRequest(
    string RowVersion,
    PartyRole PartyRole,
    PartyType PartyType,
    string? FirstName,
    string? LastName,
    string? CompanyName,
    string? Email,
    string? Phone,
    string? Notes);


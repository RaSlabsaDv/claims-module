namespace ClaimsModule.Application.Claims.Dtos;

public sealed record ClaimPartyDto(
    Guid Id,
    string PartyRole,
    string PartyType,
    string? FirstName,
    string? LastName,
    string? CompanyName,
    string? Email,
    string? Phone,
    string? Notes,
    bool IsActive
);
using ClaimsModule.Domain.Common;
using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Domain.Entities;

public sealed class ClaimParty : BaseEntity
{
    public Guid ClaimId { get; private set; }
    public Claim Claim { get; private set; } = default!;

    public PartyRole PartyRole { get; private set; }
    public PartyType PartyType { get; private set; }

    // Person fields
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }

    // Company fields
    public string? CompanyName { get; private set; }

    // Contact
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Notes { get; private set; }

    // Soft remove — IsActive замість IsDeleted (специфікація 9.3)
    public bool IsActive { get; private set; }

    // EF Core
    private ClaimParty() { }

    public static ClaimParty Create(
        Guid claimId,
        PartyRole partyRole,
        PartyType partyType,
        Guid createdByUserId,
        string? firstName = null,
        string? lastName = null,
        string? companyName = null,
        string? email = null,
        string? phone = null,
        string? notes = null)
    {
        var party = new ClaimParty
        {
            ClaimId = claimId,
            PartyRole = partyRole,
            PartyType = partyType,
            FirstName = firstName,
            LastName = lastName,
            CompanyName = companyName,
            Email = email,
            Phone = phone,
            Notes = notes,
            IsActive = true
        };

        party.SetCreated(createdByUserId);

        return party;
    }

    public void Deactivate(Guid userId)
    {
        IsActive = false;
        SetUpdated(userId);
    }

    public void Update(
        string? firstName,
        string? lastName,
        string? companyName,
        string? email,
        string? phone,
        string? notes,
        Guid updatedByUserId)
    {
        FirstName = firstName;
        LastName = lastName;
        CompanyName = companyName;
        Email = email;
        Phone = phone;
        Notes = notes;

        SetUpdated(updatedByUserId);
    }
}

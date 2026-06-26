using ClaimsModule.Domain.Common;
using ClaimsModule.Domain.Enums;
using ClaimsModule.Domain.Exceptions;
using ClaimsModule.Domain.StateMachines;

namespace ClaimsModule.Domain.Entities;

public sealed class Claim : BaseEntity
{
    // ---------------------------------------------------------------------------
    // Identity & Tenant
    // ---------------------------------------------------------------------------
    public Guid OrganisationId { get; private set; }
    public string ClaimNumber { get; private set; } = default!;

    // ---------------------------------------------------------------------------
    // Policy (nullable — unknown policy intake дозволений)
    // ---------------------------------------------------------------------------
    public Guid? PolicyId { get; private set; }
    public string? PolicyNumber { get; private set; }
    public string? ClientName { get; private set; }

    // ---------------------------------------------------------------------------
    // Status & Severity
    // ---------------------------------------------------------------------------
    public ClaimStatus Status { get; private set; }
    public ClaimSeverity Severity { get; private set; }

    // ---------------------------------------------------------------------------
    // Key Dates
    // ---------------------------------------------------------------------------
    public DateTimeOffset ReportedDate { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }

    // ---------------------------------------------------------------------------
    // Assignment & Notes
    // ---------------------------------------------------------------------------
    public Guid? AssignedHandlerId { get; private set; }
    public string? ClosureReason { get; private set; }
    public string? Notes { get; private set; }

    // ---------------------------------------------------------------------------
    // Optimistic Concurrency
    // ---------------------------------------------------------------------------
    public byte[] RowVer { get; private set; } = default!;

    // ---------------------------------------------------------------------------
    // Navigation Properties
    // ---------------------------------------------------------------------------
    private LossEvent? _lossEvent;
    private readonly List<ClaimParty> _parties = [];
    private readonly List<ClaimRiskObject> _riskObjects = [];
    private readonly List<ClaimReserveComponent> _reserves = [];
    private readonly List<ClaimDocument> _documents = [];

    public LossEvent? LossEvent => _lossEvent;
    public IReadOnlyList<ClaimParty> Parties => _parties.AsReadOnly();
    public IReadOnlyList<ClaimRiskObject> RiskObjects => _riskObjects.AsReadOnly();
    public IReadOnlyList<ClaimReserveComponent> Reserves => _reserves.AsReadOnly();
    public IReadOnlyList<ClaimDocument> Documents => _documents.AsReadOnly();

    // EF Core
    private Claim() { }

    // ---------------------------------------------------------------------------
    // Factory Method
    // ---------------------------------------------------------------------------
    public static Claim Create(
        Guid organisationId,
        string claimNumber,
        ClaimSeverity severity,
        Guid createdByUserId,
        DateTimeOffset lossDate,
        string lossDescription,
        string causeOfLossCode,
        Guid? policyId = null,
        string? policyNumber = null,
        string? clientName = null,
        Guid? assignedHandlerId = null,
        string? notes = null,
        string? lossLocation = null,
        decimal? estimatedLossAmount = null,
        string? policeReportNumber = null)
    {
        var claim = new Claim
        {
            OrganisationId = organisationId,
            ClaimNumber = claimNumber,
            PolicyId = policyId,
            PolicyNumber = policyNumber,
            ClientName = clientName,
            Status = ClaimStatus.Draft,
            Severity = severity,
            ReportedDate = DateTimeOffset.UtcNow,
            AssignedHandlerId = assignedHandlerId,
            Notes = notes
        };

        claim.SetCreated(createdByUserId);

        // LossEvent створюється агрегатом
        claim._lossEvent = LossEvent.Create(
            claimId: claim.Id,
            lossDate: lossDate,
            lossDescription: lossDescription,
            causeOfLossCode: causeOfLossCode,
            reportDate: DateTimeOffset.UtcNow,
            createdByUserId: createdByUserId,
            lossLocation: lossLocation,
            estimatedLossAmount: estimatedLossAmount,
            policeReportNumber: policeReportNumber);

        return claim;
    }

    // ---------------------------------------------------------------------------
    // Status Transitions
    // ---------------------------------------------------------------------------
    public void TransitionTo(ClaimStatus targetStatus, Guid userId, string? reason = null)
    {
        if (!ClaimStateMachine.IsValidTransition(Status, targetStatus))
            throw new DomainException(
                $"Transition from {Status} to {targetStatus} is not permitted.");

        if (targetStatus == ClaimStatus.Closed)
        {
            var blockingReasons = GetClosureBlockingReasons();
            if (blockingReasons.Count > 0)
                throw new DomainException(
                    $"Claim cannot be closed: {string.Join("; ", blockingReasons)}");
        }

        if (targetStatus == ClaimStatus.Withdrawn && string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Withdrawal reason is required.");

        if (targetStatus == ClaimStatus.Reopened && string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Reopen reason is required.");

        var previousStatus = Status;
        Status = targetStatus;

        if (targetStatus == ClaimStatus.Closed)
        {
            ClosedAt = DateTimeOffset.UtcNow;
            ClosureReason = reason;
        }

        SetUpdated(userId);

        // Специфікація: Reopened → одразу переходить в Open
        if (targetStatus == ClaimStatus.Reopened)
            TransitionTo(ClaimStatus.Open, userId);
    }

    // ---------------------------------------------------------------------------
    // Closure Conditions (CC-01, CC-03)
    // ---------------------------------------------------------------------------
    public IReadOnlyList<string> GetClosureBlockingReasons()
    {
        var reasons = new List<string>();

        var hasPendingReserves = _reserves
            .Any(r => r.Transactions.Any(t => t.ApprovalStatus == ReserveApprovalStatus.PendingApproval));

        if (hasPendingReserves)
            reasons.Add("CC-01: There are reserve components pending approval.");

        var hasClaimant = _parties.Any(p => p.IsActive && p.PartyRole == PartyRole.Claimant);
        if (!hasClaimant)
            reasons.Add("CC-03: At least one active Claimant party is required.");

        return reasons.AsReadOnly();
    }

    public bool HasOpenReservesWarning()
        => _reserves.Any(r => r.CurrentAmount > 0);

    // ---------------------------------------------------------------------------
    // Party Management
    // ---------------------------------------------------------------------------
    public void AddParty(ClaimParty party)
        => _parties.Add(party);

    public void RemoveParty(Guid partyId, Guid userId)
    {
        var party = _parties.FirstOrDefault(p => p.Id == partyId)
            ?? throw new DomainException($"Party {partyId} not found on claim.");

        if (party.PartyRole == PartyRole.Claimant)
        {
            var activeClaimants = _parties
                .Count(p => p.IsActive && p.PartyRole == PartyRole.Claimant);

            if (activeClaimants <= 1)
                throw new DomainException("Cannot remove the last Claimant from the claim.");
        }

        party.Deactivate(userId);
    }

    // ---------------------------------------------------------------------------
    // Risk Object Management
    // ---------------------------------------------------------------------------
    public void AddRiskObject(ClaimRiskObject riskObject)
    {
        if (Status is ClaimStatus.Closed or ClaimStatus.Withdrawn)
            throw new DomainException("Cannot add risk object to a closed or withdrawn claim.");

        if (riskObject.IsPrimary && _riskObjects.Any(r => r.IsPrimary && !r.IsDeleted))
            throw new DomainException("Claim already has a primary risk object.");

        _riskObjects.Add(riskObject);
    }

    // ---------------------------------------------------------------------------
    // Assignment & Notes
    // ---------------------------------------------------------------------------
    public void AssignHandler(Guid handlerId, Guid userId)
    {
        AssignedHandlerId = handlerId;
        SetUpdated(userId);
    }

    public void UpdateNotes(string notes, Guid userId)
    {
        Notes = notes;
        SetUpdated(userId);
    }

    public void AddDocument(ClaimDocument document)
    {
        if (Status is ClaimStatus.Closed or ClaimStatus.Withdrawn)
            throw new DomainException("Cannot add document to a closed or withdrawn claim.");
    
        if (_documents.Any(d => !d.IsDeleted && d.DocumentName == document.DocumentName))
            throw new DomainException($"Document '{document.DocumentName}' already exists on this claim.");
    
        _documents.Add(document);
    }
}
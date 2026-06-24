using ClaimsModule.Domain.Common;

namespace ClaimsModule.Domain.Entities;

public sealed class LossEvent : BaseEntity
{
    public Guid ClaimId { get; private set; }
    public Claim Claim { get; private set; } = default!;

    public DateTimeOffset LossDate { get; private set; }
    public string LossDescription { get; private set; } = default!;
    public string? LossLocation { get; private set; }
    public string CauseOfLossCode { get; private set; } = default!;
    public decimal? EstimatedLossAmount { get; private set; }
    public DateTimeOffset ReportDate { get; private set; }
    public string? PoliceReportNumber { get; private set; }
    public CauseOfLossCode CauseOfLossCodeRef { get; private set; } = default!;

    // EF Core
    private LossEvent() { }

    public static LossEvent Create(
        Guid claimId,
        DateTimeOffset lossDate,
        string lossDescription,
        string causeOfLossCode,
        DateTimeOffset reportDate,
        Guid createdByUserId,
        string? lossLocation = null,
        decimal? estimatedLossAmount = null,
        string? policeReportNumber = null)
    {
        var lossEvent = new LossEvent
        {
            ClaimId = claimId,
            LossDate = lossDate,
            LossDescription = lossDescription,
            CauseOfLossCode = causeOfLossCode,
            ReportDate = reportDate,
            LossLocation = lossLocation,
            EstimatedLossAmount = estimatedLossAmount,
            PoliceReportNumber = policeReportNumber
        };

        lossEvent.SetCreated(createdByUserId);

        return lossEvent;
    }

    public void Update(
        DateTimeOffset lossDate,
        string lossDescription,
        string causeOfLossCode,
        Guid updatedByUserId,
        string? lossLocation = null,
        decimal? estimatedLossAmount = null,
        string? policeReportNumber = null)
    {
        LossDate = lossDate;
        LossDescription = lossDescription;
        CauseOfLossCode = causeOfLossCode;
        LossLocation = lossLocation;
        EstimatedLossAmount = estimatedLossAmount;
        PoliceReportNumber = policeReportNumber;

        SetUpdated(updatedByUserId);
    }
}

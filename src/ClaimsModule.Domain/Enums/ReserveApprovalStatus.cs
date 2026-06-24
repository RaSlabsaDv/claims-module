namespace ClaimsModule.Domain.Enums;

public enum ReserveApprovalStatus
{
    PendingApproval,
    AutoApproved,
    Approved,
    Rejected,
    Cancelled  // retracted by submitter
}

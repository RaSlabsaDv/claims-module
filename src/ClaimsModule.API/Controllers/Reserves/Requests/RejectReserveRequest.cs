namespace ClaimsModule.API.Controllers.Reserves;

public sealed record RejectReserveRequest(
    Guid TransactionId,
    string RejectionReason,
    byte[] RowVersion);
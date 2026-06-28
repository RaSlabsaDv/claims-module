namespace ClaimsModule.API.Controllers.Reserves;

public sealed record ApproveReserveRequest(
    Guid TransactionId,
    string RowVersion);
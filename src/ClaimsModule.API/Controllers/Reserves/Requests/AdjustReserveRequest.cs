namespace ClaimsModule.API.Controllers.Reserves;

public sealed record AdjustReserveRequest(
    decimal Amount,
    string ChangeReason,
    byte[] RowVersion);
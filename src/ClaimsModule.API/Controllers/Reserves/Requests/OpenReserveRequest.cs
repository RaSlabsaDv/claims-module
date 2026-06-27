using ClaimsModule.Domain.Enums;

namespace ClaimsModule.API.Controllers.Reserves;

public sealed record OpenReserveRequest(
    byte[] RowVersion,
    ReserveComponentType ComponentType,
    decimal Amount,
    string ChangeReason,
    string? Notes);
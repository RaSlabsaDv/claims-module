using MediatR;

namespace ClaimsModule.Application.Reserves.Commands.AdjustReserve;

public sealed record AdjustReserveCommand(
    Guid ReserveId,
    decimal Amount,
    string ChangeReason) : IRequest<Unit>;
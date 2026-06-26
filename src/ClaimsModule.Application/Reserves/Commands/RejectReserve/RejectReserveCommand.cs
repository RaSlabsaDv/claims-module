using MediatR;

namespace ClaimsModule.Application.Reserves.Commands.RejectReserve;

public sealed record RejectReserveCommand(
    Guid ReserveId,
    Guid TransactionId,
    string RejectionReason) : IRequest<Unit>;
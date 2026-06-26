using MediatR;

namespace ClaimsModule.Application.Reserves.Commands.ApproveReserve;

public sealed record ApproveReserveCommand(
    Guid ReserveId,
    Guid TransactionId,
    byte[] RowVersion) : IRequest<Unit>;
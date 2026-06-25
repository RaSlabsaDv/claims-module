using ClaimsModule.Domain.Enums;
using MediatR;

namespace ClaimsModule.Application.Reserves.Commands.OpenReserve;

public sealed record OpenReserveCommand
(
    Guid ClaimId,
    ReserveComponentType ComponentType,
    decimal Amount,
    string ChangeReason,
    string? Notes) : IRequest<Guid>;
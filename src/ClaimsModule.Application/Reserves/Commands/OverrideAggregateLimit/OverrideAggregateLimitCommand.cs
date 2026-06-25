using MediatR;

namespace ClaimsModule.Application.Reserves.Commands.OverrideAggregateLimit;

public sealed record OverrideAggregateLimitCommand(
    Guid ReserveId) : IRequest<Unit>;
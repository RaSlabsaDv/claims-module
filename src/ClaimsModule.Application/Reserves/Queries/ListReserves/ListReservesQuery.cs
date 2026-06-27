using ClaimsModule.Application.Reserves.Dtos;
using MediatR;

namespace ClaimsModule.Application.Reserves.Queries.ListReserves;

public sealed record ListReservesQuery(Guid ClaimId) : IRequest<IReadOnlyList<ReserveComponentDto>>;
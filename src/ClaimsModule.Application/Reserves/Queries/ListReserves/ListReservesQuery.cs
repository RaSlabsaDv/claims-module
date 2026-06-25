using ClaimsModule.Application.Reserves.Dtos;
using MediatR;

public sealed record ListReservesQuery(Guid ClaimId) : IRequest<IReadOnlyList<ReserveComponentDto>>;
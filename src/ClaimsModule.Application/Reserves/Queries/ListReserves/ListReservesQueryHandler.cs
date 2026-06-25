using AutoMapper;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Application.Reserves.Dtos;
using MediatR;

namespace ClaimsModule.Application.Reserves.Queries.ListReserves;

public sealed class ListReservesQueryHandler
(
    IReserveRepository reserveRepository,
    IMapper mapper
) : IRequestHandler<ListReservesQuery, IReadOnlyList<ReserveComponentDto>>
{
    public async Task<IReadOnlyList<ReserveComponentDto>> Handle(ListReservesQuery request, CancellationToken ct)
    {
        var components = await reserveRepository.ListByClaimAsync(request.ClaimId, ct);

        return mapper.Map<IReadOnlyList<ReserveComponentDto>>(components);
    }
}
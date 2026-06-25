using AutoMapper;
using ClaimsModule.Application.Common.Interfaces;
using MediatR;

namespace ClaimsModule.Application.Claims.Queries.ListClaims;

public sealed class ListClaimsQueryHandler
(
    IClaimRepository claimRepository,
    IMapper mapper) 
        : IRequestHandler<ListClaimsQuery, ListClaimsResult>
{
    public async Task<ListClaimsResult> Handle
    (   ListClaimsQuery request, 
        CancellationToken ct)
    {
        var (claims, totalCount) = await claimRepository.ListAsync
        (
            request.Filter,
            request.Page,
            request.PageSize,
            ct
        );

        var items = mapper.Map<IReadOnlyList<ClaimListItemDto>>(claims);

        return new ListClaimsResult(items, totalCount);
    }
}
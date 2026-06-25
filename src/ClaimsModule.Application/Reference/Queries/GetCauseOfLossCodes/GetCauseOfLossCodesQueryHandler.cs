using ClaimsModule.Application.Common.Interfaces;
using MediatR;

namespace ClaimsModule.Application.Reference.Queries.GetCauseOfLossCodes;

public sealed class GetCauseOfLossCodesQueryHandler
(
    ICauseOfLossCodeRepository causeOfLossCodeRepository
) : IRequestHandler<GetCauseOfLossCodesQuery, IReadOnlyList<CauseOfLossCodeDto>>
{
    public async Task<IReadOnlyList<CauseOfLossCodeDto>> Handle(GetCauseOfLossCodesQuery request, CancellationToken ct)
    {
        var codes = await causeOfLossCodeRepository.ListActiveAsync(ct);

        return codes.Select(c => new CauseOfLossCodeDto(
            Id: c.Id,
            Code: c.Code,
            Name: c.Name,
            PerilCategory: c.PerilCategory,
            SortOrder: c.SortOrder
        )).ToList();
    }
}
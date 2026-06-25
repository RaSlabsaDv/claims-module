using MediatR;

namespace ClaimsModule.Application.Reference.Queries.GetCauseOfLossCodes;

public sealed record GetCauseOfLossCodesQuery() : IRequest<IReadOnlyList<CauseOfLossCodeDto>>;
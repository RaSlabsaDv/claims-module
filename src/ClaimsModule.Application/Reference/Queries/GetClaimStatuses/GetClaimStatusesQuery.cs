using MediatR;

namespace ClaimsModule.Application.Reference.Queries.GetClaimStatuses;

public sealed record GetClaimStatusesQuery() : IRequest<IReadOnlyList<ClaimStatusDto>>;
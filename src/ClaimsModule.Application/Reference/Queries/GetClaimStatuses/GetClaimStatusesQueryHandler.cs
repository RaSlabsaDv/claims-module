using ClaimsModule.Domain.Enums;
using ClaimsModule.Domain.StateMachines;
using MediatR;

namespace ClaimsModule.Application.Reference.Queries.GetClaimStatuses;

public sealed class GetClaimStatusesQueryHandler()
: IRequestHandler<GetClaimStatusesQuery, IReadOnlyList<ClaimStatusDto>>
{
    public Task<IReadOnlyList<ClaimStatusDto>> Handle(GetClaimStatusesQuery request, CancellationToken ct)
    {
        var statuses = Enum.GetValues<ClaimStatus>()
            .Select(s => new ClaimStatusDto(
                Status: s,
                ValidTransitions: ClaimStateMachine.GetValidTransitions(s)))
            .ToList();

        return Task.FromResult<IReadOnlyList<ClaimStatusDto>>(statuses);
    }
}
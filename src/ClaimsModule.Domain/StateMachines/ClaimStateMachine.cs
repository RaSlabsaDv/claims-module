using ClaimsModule.Domain.Enums;

namespace ClaimsModule.Domain.StateMachines;

public static class ClaimStateMachine
{
    public static IReadOnlyList<ClaimStatus> GetValidTransitions(ClaimStatus from) =>
        from switch
        {
            ClaimStatus.Draft              => [ClaimStatus.Open],
            ClaimStatus.Open               => [ClaimStatus.UnderInvestigation, ClaimStatus.PendingPayment, ClaimStatus.Closed, ClaimStatus.Withdrawn],
            ClaimStatus.UnderInvestigation => [ClaimStatus.Open, ClaimStatus.PendingPayment, ClaimStatus.Closed, ClaimStatus.Withdrawn],
            ClaimStatus.PendingPayment     => [ClaimStatus.Closed],
            ClaimStatus.Closed             => [ClaimStatus.Reopened],
            ClaimStatus.Reopened           => [ClaimStatus.Open],
            ClaimStatus.Withdrawn          => [],
            _                              => []
        };

    public static bool IsValidTransition(ClaimStatus from, ClaimStatus to)
        => GetValidTransitions(from).Contains(to);
}
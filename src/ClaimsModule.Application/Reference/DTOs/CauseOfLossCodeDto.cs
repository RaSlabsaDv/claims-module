namespace ClaimsModule.Application.Reference.Queries.GetCauseOfLossCodes;

public sealed record CauseOfLossCodeDto(
    Guid Id,
    string Code,
    string Name,
    string PerilCategory,
    int SortOrder
);
namespace ClaimsModule.API.Controllers.Policies;

public sealed record SearchPoliciesRequest(
    string SearchTerm,
    int MaxResults = 10);
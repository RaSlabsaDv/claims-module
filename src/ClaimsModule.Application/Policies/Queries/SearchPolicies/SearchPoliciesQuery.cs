using ClaimsModule.Application.Policies.Dtos;
using MediatR;

namespace ClaimsModule.Application.Policies.Queries.SearchPolicies;

public sealed record SearchPoliciesQuery
(
    string SearchTerm, 
    int MaxResults
) : IRequest<IReadOnlyList<PolicyDto>>;
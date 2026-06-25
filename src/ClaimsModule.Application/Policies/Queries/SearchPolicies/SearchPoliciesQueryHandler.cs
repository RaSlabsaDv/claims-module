using System.Text.Json;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Application.Policies.Dtos;
using MediatR;

namespace ClaimsModule.Application.Policies.Queries.SearchPolicies;

public sealed class SearchPoliciesQueryHandler
(
    IPolicyRepository policyRepository
) : IRequestHandler<SearchPoliciesQuery, IReadOnlyList<PolicyDto>>
{
    public async Task<IReadOnlyList<PolicyDto>> Handle(SearchPoliciesQuery request, CancellationToken ct)
    {
        var policies = await policyRepository.SearchAsync(request.SearchTerm, request.MaxResults, ct);

        var dtos = policies.Select(p => new PolicyDto(
            Id: p.Id,
            PolicyNumber: p.PolicyNumber,
            ClientName: p.ClientName,
            EffectiveDate: p.EffectiveDate,
            ExpirationDate: p.ExpirationDate,
            Status: p.Status,
            CoverageTypes: JsonSerializer.Deserialize<List<string>>(p.CoverageTypes) ?? []
        ))
        .ToList();

        return dtos;
    }
}
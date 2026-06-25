using System.Text.Json;
using ClaimsModule.Application.Common.Interfaces;
using ClaimsModule.Application.Policies.Dtos;
using ClaimsModule.Domain.Entities;
using ClaimsModule.Domain.Exceptions;
using MediatR;

namespace ClaimsModule.Application.Policies.Queries.GetPolicyById;

public sealed class GetPolicyByIdQueryHandler
(
    IPolicyRepository policyRepository)
    : IRequestHandler<GetPolicyByIdQuery, PolicyDto>
{
    public async Task<PolicyDto> Handle(GetPolicyByIdQuery request, CancellationToken ct)
    {
        var policy = await policyRepository.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException(nameof(Policy), request.Id);

        return new PolicyDto(
            Id: policy.Id,
            PolicyNumber: policy.PolicyNumber,
            ClientName: policy.ClientName,
            EffectiveDate: policy.EffectiveDate,
            ExpirationDate: policy.ExpirationDate,
            Status: policy.Status,
            CoverageTypes: JsonSerializer.Deserialize<List<string>>(policy.CoverageTypes) ?? []
        );
    }
}
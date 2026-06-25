using ClaimsModule.Application.Policies.Dtos;
using MediatR;

namespace ClaimsModule.Application.Policies.Queries.GetPolicyById;

public sealed record GetPolicyByIdQuery(Guid Id) : IRequest<PolicyDto>;
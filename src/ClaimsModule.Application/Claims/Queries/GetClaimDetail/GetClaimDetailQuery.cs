using ClaimsModule.Application.Claims.Dtos;
using MediatR;

namespace ClaimsModule.Application.Claims.Queries.GetClaimDetail;

public sealed record GetClaimDetailQuery(Guid id) : IRequest<ClaimDetailDto>;
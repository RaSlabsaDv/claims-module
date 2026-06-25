using ClaimsModule.Application.Claims.Dtos;
using MediatR;

public sealed record GetClaimDetailQuery(Guid id) : IRequest<ClaimDetailDto>;
using ClaimsModule.Application.Claims.Queries.ListClaims;
using ClaimsModule.Application.Common.Interfaces;
using MediatR;

public sealed record ListClaimsQuery(
    ClaimListFilter Filter,
    int Page,
    int PageSize) : IRequest<ListClaimsResult>;
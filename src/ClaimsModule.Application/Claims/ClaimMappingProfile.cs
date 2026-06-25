using AutoMapper;
using ClaimsModule.Application.Claims.Queries.ListClaims;
using ClaimsModule.Domain.Entities;

public sealed class ClaimMappingProfile : Profile
{
    public ClaimMappingProfile()
    {
        CreateMap<Claim, ClaimListItemDto>();
    }
}
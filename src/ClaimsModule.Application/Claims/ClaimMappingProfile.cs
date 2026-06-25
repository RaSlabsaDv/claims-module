using AutoMapper;
using ClaimsModule.Application.Claims.Dtos;
using ClaimsModule.Application.Claims.Queries.ListClaims;
using ClaimsModule.Domain.Entities;

namespace ClaimsModule.Application.Claims;

public sealed class ClaimMappingProfile : Profile
{
    public ClaimMappingProfile()
    {
        CreateMap<Claim, ClaimListItemDto>();
        CreateMap<ClaimDocument, ClaimDocumentDto>();
        CreateMap<ClaimParty, ClaimPartyDto>();
        CreateMap<ClaimRiskObject, ClaimRiskObjectDto>();
        CreateMap<LossEvent, LossEventDto>();

        CreateMap<ClaimDocument, ClaimDocumentDto>()
            .ForMember(dst => dst.DownloadUrl, opt => opt.Ignore());
    }
}
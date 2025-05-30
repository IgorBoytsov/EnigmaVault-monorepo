using AutoMapper;
using EnigmaVault.Application.Dtos;
using EnigmaVault.Domain.DomainModels;

namespace EnigmaVault.Application.AutoMappers.Profiles
{
    public class GenderProfile : Profile
    {
        public GenderProfile()
        {
            CreateMap<GenderDto, GenderDomain>()
                .ConstructUsing(dto => GenderDomain.Reconstitute(dto.IdGender, dto.GenderName));

            CreateMap<GenderDomain, GenderDto>();
        }
    }
}
using AutoMapper;
using EnigmaVault.Application.Dtos;
using EnigmaVault.Domain.DomainModels;

namespace EnigmaVault.Application.AutoMappers.Profiles
{
    public class IconProfile : Profile
    {
        public IconProfile()
        {
            CreateMap<IconDto, IconDomain>()
                .ConstructUsing(dto => IconDomain.Reconstruct(dto.IdIcon, dto.IdUser, dto.SvgCode, dto.IconName, dto.IdIconCategory, dto.IsCommon));

            CreateMap<IconDomain, IconDto>();
        }
    }
}
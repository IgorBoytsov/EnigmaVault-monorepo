using AutoMapper;
using EnigmaVault.Application.Dtos;
using EnigmaVault.Domain.DomainModels;

namespace EnigmaVault.Application.AutoMappers.Profiles
{
    public class CountryProfile : Profile
    {
        public CountryProfile()
        {
            CreateMap<CountryDto, CountryDomain>()
                .ConstructUsing(dto => CountryDomain.Reconstitute(dto.IdCountry, dto.CountryName));

            CreateMap<CountryDomain, CountryDto>();
        }
    }
}
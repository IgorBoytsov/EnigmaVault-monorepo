using EnigmaVault.Application.Dtos;
using EnigmaVault.Domain.DomainModels;

namespace EnigmaVault.Application.Mappers
{
    public static class CountriesMapper
    {
        public static CountryDto ToDto(this CountryDomain countryDomain)
        {
            return new CountryDto
            {
                IdCountry = countryDomain.IdCountry,
                CountryName = countryDomain.CountryName,
            };
        }
    }
}
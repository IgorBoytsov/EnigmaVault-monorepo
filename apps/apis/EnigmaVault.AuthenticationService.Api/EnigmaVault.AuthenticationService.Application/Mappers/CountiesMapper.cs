using EnigmaVault.AuthenticationService.Application.DTOs;
using EnigmaVault.AuthenticationService.Domain.DomainModels;

namespace EnigmaVault.AuthenticationService.Application.Mappers
{
    public static class CountiesMapper
    {
        public static CountryDto ToDto(this CountryDomain domain)
        {
            return new CountryDto
            {
                IdCountry = domain.IdCountry,
                CountryName = domain.CountryName,
            };
        }
    }
}
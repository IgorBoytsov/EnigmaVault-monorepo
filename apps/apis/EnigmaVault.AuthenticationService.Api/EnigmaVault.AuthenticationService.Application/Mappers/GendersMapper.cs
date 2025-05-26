using EnigmaVault.AuthenticationService.Application.DTOs;
using EnigmaVault.AuthenticationService.Domain.DomainModels;

namespace EnigmaVault.AuthenticationService.Application.Mappers
{
    public static class GendersMapper
    {
        public static GenderDto ToDto(this GenderDomain domain)
        {
            return new GenderDto
            {
                IdGender = domain.IdGender,
                GenderName = domain.GenderName,
            };
        }
    }
}
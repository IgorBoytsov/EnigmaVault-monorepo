using EnigmaVault.Application.Dtos;
using EnigmaVault.Domain.DomainModels;

namespace EnigmaVault.Application.Mappers
{
    public static class GendersMapper
    {
        public static GenderDto ToDto(this GenderDomain genderDomain)
        {
            return new GenderDto
            {
                IdGender = genderDomain.IdGender,
                GenderName = genderDomain.GenderName,
            };
        }
    }
}
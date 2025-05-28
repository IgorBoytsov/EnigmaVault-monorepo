using EnigmaVault.Application.Dtos;
using EnigmaVault.Domain.DomainModels;

namespace EnigmaVault.Application.Mappers
{
    public static class UsersMapper
    {
        public static UserDto ToDto(this UserDomain userDomain)
        {
            return new UserDto
            {
                IdUser = userDomain.IdUser,
                Login = userDomain.Login,
                UserName = userDomain.UserName,
                Phone = userDomain.Phone,
                Email = userDomain.Email,
                IdGender = userDomain.IdGender,
                IdCountry = userDomain.IdCountry,
            };
        }
    }
}
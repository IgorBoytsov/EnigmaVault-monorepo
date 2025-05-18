using EnigmaVault.AuthenticationService.Application.DTOs;
using EnigmaVault.AuthenticationService.Domain.DomainModels;

namespace EnigmaVault.AuthenticationService.Application.Mappers
{
    public static class UsersMapper
    {
        public static UserDto ToDto(this UserDomain user)
        {
            return new UserDto
            {
                IdUser = user.IdUser,
                Login = user.Login,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                IdCountry = user.IdCountry,
                IdGender = user.IdGender,
            };
        }
    }
}
using EnigmaVault.AuthenticationService.Api.DTOs.Requests;
using EnigmaVault.AuthenticationService.Application.DTOs;

namespace EnigmaVault.AuthenticationService.Api.Mappers
{
    public static class RegisterMapper
    {
        public static RegisterUserCommand ToMapCommand(this RegisterUserApiRequest request)
        {
            return new RegisterUserCommand
            {
                Login = request.Login,
                UserName = request.UserName,
                Password = request.Password,
                Email = request.Email,
                Phone = request.Phone,
                IdGender = request.IdGender,
                IdCountry = request.IdCountry,
            };
        }
    }
}
using EnigmaVault.AuthenticationService.Api.DTOs.Requests;
using EnigmaVault.AuthenticationService.Application.DTOs.Commands;

namespace EnigmaVault.AuthenticationService.Api.Mappers
{
    public static class AuthenticationMapper
    {
        public static AuthenticateUserCommand ToMapCommand(this AuthenticateUserApiRequest request)
        {
            return new AuthenticateUserCommand
            {
                Login = request.Login,
                Password = request.Password,
            };
        }
    }
}
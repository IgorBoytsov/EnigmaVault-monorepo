using EnigmaVault.AuthenticationService.Api.DTOs.Requests;
using EnigmaVault.AuthenticationService.Application.DTOs.Commands;

namespace EnigmaVault.AuthenticationService.Api.Mappers
{
    public static class RecoveryMapper
    {
        public static RecoveryAccessUserCommand ToCommand(this RecoveryAccessUserApiRequest reauest)
        {
            return new RecoveryAccessUserCommand
            {
                Login = reauest.Login,
                Email = reauest.Email,
                NewPassword = reauest.NewPassword,
            };
        }
    }
}
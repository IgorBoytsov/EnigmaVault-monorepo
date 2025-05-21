using EnigmaVault.AuthenticationService.Application.DTOs.Commands;
using EnigmaVault.AuthenticationService.Application.DTOs.Results;

namespace EnigmaVault.AuthenticationService.Application.Abstractions.UseCases
{
    public interface IAuthenticateUserUseCase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task<UserResult> AuthenticateAsync(AuthenticateUserCommand command);
    }
}
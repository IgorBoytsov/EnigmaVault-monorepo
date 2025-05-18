using EnigmaVault.AuthenticationService.Application.DTOs;

namespace EnigmaVault.AuthenticationService.Application.Abstractions.UseCases
{
    public interface IRegisterUserUseCase
    {
        Task<RegisterUserResult> RegisterAsync(RegisterUserCommand command);
    }
}
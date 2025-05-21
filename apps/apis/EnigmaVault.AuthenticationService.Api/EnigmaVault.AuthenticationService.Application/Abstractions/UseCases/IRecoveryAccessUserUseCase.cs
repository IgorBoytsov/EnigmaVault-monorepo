using EnigmaVault.AuthenticationService.Application.DTOs.Commands;
using EnigmaVault.AuthenticationService.Application.DTOs.Results;

namespace EnigmaVault.AuthenticationService.Application.Abstractions.UseCases
{
    public interface IRecoveryAccessUserUseCase
    {
        Task<UserResult> RecoveryAccessAsync(RecoveryAccessUserCommand command);
    }
}
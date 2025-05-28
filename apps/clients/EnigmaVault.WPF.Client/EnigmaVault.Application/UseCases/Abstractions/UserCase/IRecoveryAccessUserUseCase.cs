using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.UserCase
{
    public interface IRecoveryAccessUserUseCase
    {
        Task<Result<(string Login, string NewPassword)?>> RecoveryAccessAsync(string login, string email, string newPassword);
    }
}
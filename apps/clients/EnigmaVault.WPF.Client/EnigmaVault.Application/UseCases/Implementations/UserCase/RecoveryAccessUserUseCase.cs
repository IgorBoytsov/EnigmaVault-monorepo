using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.UseCases.Abstractions.UserCase;
using EnigmaVault.Domain.Enums;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Implementations.UserCase
{
    public class RecoveryAccessUserUseCase(IUserRepository userRepository) : IRecoveryAccessUserUseCase
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<(string Login, string NewPassword)?>> RecoveryAccessAsync(string login, string email, string newPassword)
        {
            var validationErrors = new List<Error>();

            if (string.IsNullOrEmpty(login))
                validationErrors.Add(new Error(ErrorCode.EmptyValue, "Вы не заполнили поле с логином."));
            if (string.IsNullOrEmpty(email))
                validationErrors.Add(new Error(ErrorCode.EmptyValue, "Вы не заполнили поле с почтой."));
            if (string.IsNullOrEmpty(newPassword))
                validationErrors.Add(new Error(ErrorCode.EmptyValue, "Вы не заполнили поле с новым паролем."));

            if (validationErrors.Any())
                return Result<ValueTuple<string, string>?>.Failure(validationErrors);

            Result<ValueTuple<string, string>?> recoveryResult = await _userRepository.RecoveryAccessAsync(login, email, newPassword);

            return recoveryResult;
        }
    }
}
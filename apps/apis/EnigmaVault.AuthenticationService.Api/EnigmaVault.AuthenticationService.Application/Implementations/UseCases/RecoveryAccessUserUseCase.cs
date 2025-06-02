using EnigmaVault.AuthenticationService.Application.Abstractions.Hashers;
using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Application.Abstractions.UseCases;
using EnigmaVault.AuthenticationService.Application.DTOs.Commands;
using EnigmaVault.AuthenticationService.Application.DTOs.Results;
using EnigmaVault.AuthenticationService.Application.Enums;
using EnigmaVault.AuthenticationService.Domain.DomainModels.Validations;
using EnigmaVault.AuthenticationService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace EnigmaVault.AuthenticationService.Application.Implementations.UseCases
{
    public class RecoveryAccessUserUseCase : IRecoveryAccessUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<RecoveryAccessUserUseCase> _logger;

        public RecoveryAccessUserUseCase(IUserRepository userRepository,
                                         IPasswordHasher passwordHasher,
                                         ILogger<RecoveryAccessUserUseCase> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<UserResult> RecoveryAccessAsync(RecoveryAccessUserCommand command)
        {   
            if (command is null)
            {
                _logger.LogError("Команда пришла без значений. {@command}", command);
                throw new ArgumentNullException(nameof(command));
            }
                
            _logger.LogInformation("<<======Начало выполнения RecoveryAccessAsync для Login: {Login} ======>>", command.Login);

            var validationErrors = new List<string>();

            #region Валидация полей

            if (string.IsNullOrEmpty(command.Login))
                validationErrors.Add("Вы не указали логин.");

            if (string.IsNullOrEmpty(command.Email))
                validationErrors.Add("Вы не указали почту.");

            if (string.IsNullOrEmpty(command.NewPassword))
                validationErrors.Add("Вы не указали пароль.");

            #endregion

            #region ValueObjects

            ValidationResults? passwordResult = Password.TryCreate(command.NewPassword, out Password? passwordVo);

            if (!passwordResult.IsValid)
                validationErrors.AddRange(passwordResult.Errors);

            #endregion

            if (validationErrors.Any())
                return UserResult.ValidationFailureResult(validationErrors);

            #region Проверки через БД

            if (!await _userRepository.ExistsByLoginAsync(command.Login))
                return UserResult.FailureResult(ErrorCode.LoginNotExist, "Логина не существует.");

            if (!await _userRepository.ExistsByEmailAsync(command.Email))
                return UserResult.FailureResult(ErrorCode.EmailNotExist, "Такая почта не зарегистрирована в системе.");

            #endregion

            var newHash = _passwordHasher.HashPassword(command.NewPassword);

            var updateResult = await _userRepository.UpdatePasswordAsync(command.Login, command.Email, newHash);

            if (updateResult)
            {
                _logger.LogInformation("<<======Выполнения RecoveryAccessAsync прошло успешно для Login: {Login} ======>>", command.Login);
                return UserResult.SuccessResult();
            }
            else
            {
                _logger.LogError("Не удалось выполнить изменение пароля для пользователя {Login}.", command.Login);
                return UserResult.FailureResult(ErrorCode.SaveUserError, "Не удалось сохранить пароль.");
            }
                
        }
    }
}
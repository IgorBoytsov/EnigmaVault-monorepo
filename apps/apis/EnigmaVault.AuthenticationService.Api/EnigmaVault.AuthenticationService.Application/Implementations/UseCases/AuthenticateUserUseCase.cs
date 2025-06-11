using EnigmaVault.AuthenticationService.Application.Abstractions.Hashers;
using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Application.Abstractions.UseCases;
using EnigmaVault.AuthenticationService.Application.DTOs.Commands;
using EnigmaVault.AuthenticationService.Application.DTOs.Results;
using EnigmaVault.AuthenticationService.Application.Enums;
using EnigmaVault.AuthenticationService.Application.Mappers;
using Microsoft.Extensions.Logging;

namespace EnigmaVault.AuthenticationService.Application.Implementations.UseCases
{
    public class AuthenticateUserUseCase : IAuthenticateUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<AuthenticateUserUseCase> _logger;

        public AuthenticateUserUseCase(IUserRepository userRepository,
                                       IPasswordHasher passwordHasher,
                                       ILogger<AuthenticateUserUseCase> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public async Task<UserResult> AuthenticateAsync(AuthenticateUserCommand command)
        {
            if (command is null)
            {
                _logger.LogError("Команда пришла без значений. {@command}", command);
                throw new ArgumentNullException($"Значение {nameof(command)} было пустым.");
            }

            _logger.LogInformation("<<======Начало выполнения AuthenticateUserUseCase для Login: {Login} ======>>", command.Login);

            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(command.Login))
                validationErrors.Add("Вы не указали логин.");
            if (string.IsNullOrWhiteSpace(command.Password))
                validationErrors.Add("Вы не указали пароль");

            if (validationErrors.Any())
                return UserResult.ValidationFailureResult(validationErrors, "Ошибки валидации");

            if (!await _userRepository.ExistsByLoginAsync(command.Login))
                return UserResult.FailureResult(ErrorCode.LoginNotExist, $"Логин {command.Login} не зарегистрирован.");

            try
            {
                var userDomain = await _userRepository.GetUserByLoginAsync(command.Login);

                if (userDomain is null) 
                    return UserResult.FailureResult(ErrorCode.LoginNotExist, "Пользователь не был найден во время процесса аутентификации.");

                var verifiablePassword = _passwordHasher.VerifyPassword(command.Password, userDomain.PasswordHash);

                if (!verifiablePassword)
                    return UserResult.FailureResult(ErrorCode.InvalidPassword, "Указан не верный пароль.");

                await _userRepository.UpdateDateEntryAsync(userDomain.IdUser);
                var argon2Params = _passwordHasher.GetParametersFromHash(userDomain.PasswordHash);

                _logger.LogInformation("<<======Конец выполнения AuthenticateUserUseCase. AuthenticateUserUseCase успешно завершен для Login: {Login}======>>", command.Login);
                return UserResult.SuccessResult(userDomain.ToDto(), argon2Params);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Непредвиденная ошибка в AuthenticateUserUseCase при обработке Login: {Login}", command.Login);
                return UserResult.FailureResult(ErrorCode.UnknownError, "Во время аутентификации произошла непредвиденная ошибка");
            }
        }
    }
}
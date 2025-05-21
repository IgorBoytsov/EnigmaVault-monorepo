using EnigmaVault.AuthenticationService.Application.Abstractions.Hashers;
using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Application.Abstractions.UseCases;
using EnigmaVault.AuthenticationService.Application.DTOs.Commands;
using EnigmaVault.AuthenticationService.Application.DTOs.Results;
using EnigmaVault.AuthenticationService.Application.Enums;
using EnigmaVault.AuthenticationService.Application.Mappers;
using System.Diagnostics;

namespace EnigmaVault.AuthenticationService.Application.Implementations.UseCases
{
    public class AuthenticateUserUseCase : IAuthenticateUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AuthenticateUserUseCase(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserResult> AuthenticateAsync(AuthenticateUserCommand command)
        {
            if (command is null)
                throw new ArgumentNullException($"Значение {nameof(command)} было пустым.");

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
                return UserResult.SuccessResult(userDomain.ToDto());

            }
            catch (Exception ex)
            {
                //TODO: Сделать логирование ошибки
                Debug.WriteLine(ex.Message);
                return UserResult.FailureResult(ErrorCode.UnknownError, "Во время аутентификации произошла непредвиденная ошибка");
            }
        }
    }
}
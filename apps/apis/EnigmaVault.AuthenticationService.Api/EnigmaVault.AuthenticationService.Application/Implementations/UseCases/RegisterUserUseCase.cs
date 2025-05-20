using EnigmaVault.AuthenticationService.Application.Abstractions.Hashers;
using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Application.Abstractions.UseCases;
using EnigmaVault.AuthenticationService.Application.DTOs.Commands;
using EnigmaVault.AuthenticationService.Application.DTOs.Results;
using EnigmaVault.AuthenticationService.Application.Enums;
using EnigmaVault.AuthenticationService.Application.Mappers;
using EnigmaVault.AuthenticationService.Domain.Constants;
using EnigmaVault.AuthenticationService.Domain.DomainModels;
using EnigmaVault.AuthenticationService.Domain.Enums;
using EnigmaVault.AuthenticationService.Domain.ValueObjects;

namespace EnigmaVault.AuthenticationService.Application.Implementations.UseCases
{
    public class RegisterUserUseCase : IRegisterUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _argon2idPasswordHasher;

        public RegisterUserUseCase(IUserRepository userRepository,
                                   IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _argon2idPasswordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        /*--Создание пользователя-------------------------------------------------------------------------*/

        public async Task<UserResult> RegisterAsync(RegisterUserCommand command)
        {
            var validationErrors = new List<string>();

            #region Валидация пустых строк 

            if (string.IsNullOrWhiteSpace(command.Login))
                validationErrors.Add("Вы не заполнили поле с логином");
            if (string.IsNullOrWhiteSpace(command.UserName))
                validationErrors.Add("Вы не заполнили поле с именем пользователя");
            if (string.IsNullOrWhiteSpace(command.Password) || command.Password.Length < UserConstants.MinUserPasswordLength)
                validationErrors.Add($"Длинна пароля должна быть больше {UserConstants.MinUserPasswordLength}");

            #endregion

            #region Валидация через ValueObject

            var userLoginValidationResult = Login.TryCreate(command.Login, out Login? loginVo);

            if (!userLoginValidationResult.IsValid)
                validationErrors.AddRange(userLoginValidationResult.Errors);

            var passwordResult = Password.TryCreate(command.Password, out Password? passwordVo);

            if (!passwordResult.IsValid)
                validationErrors.AddRange(passwordResult.Errors);

            var emailValidationResult = EmailAddress.TryCreate(command.Email, out EmailAddress? emailVo);

            if (!emailValidationResult.IsValid)
                validationErrors.AddRange(emailValidationResult.Errors);

            var phoneValidationResult = PhoneNumber.TryCreate(command.Phone, out PhoneNumber? phoneVo);

            if (!phoneValidationResult.IsValid)
                validationErrors.AddRange(emailValidationResult.Errors);

            #endregion

            if (validationErrors.Any())
                return UserResult.ValidationFailureResult(validationErrors, "Ошибки валидации.");

            #region Валидация через БД

            if (loginVo != null && await _userRepository.ExistsByLoginAsync(command.Login))
                return UserResult.FailureResult(ErrorCode.LoginAlreadyTaken, $"Логин {command.Login} уже занят, придумайте другой.");

            if (emailVo != null && await _userRepository.ExistsByEmailAsync(command.Email))
                return UserResult.FailureResult(ErrorCode.EmailAlreadyRegistered, $"Пользователь с таким адресом электронной почты: {command.Email} уже существует. Если не помните пароль, то попробуйте восстановить его.");

            if (emailVo != null && await _userRepository.ExistsByPhoneAsync(command.Phone))
                return UserResult.FailureResult(ErrorCode.PhoneAlreadyRegistered, $"Пользователь с таким номером телефона: {command.Phone} уже существует. Если не помните пароль, то попробуйте восстановить его.");

            #endregion

            #region Хеширование пароля

            string passwordHash = _argon2idPasswordHasher.HashPassword(passwordVo!.Value);

            #endregion

            #region Выставление значений не зависящих от пользователя

            const int defaultRoleId = (int)Roles.User;
            const int defaultStatusAccountId = (int)AccountStatus.Available;

            #endregion

            #region Создание и проверка домейн сущьности

            var (Domain, Message) = UserDomain.Create(
                loginVo!,
                command.UserName,
                passwordHash,
                emailVo!,
                phoneVo,
                defaultStatusAccountId,
                command.IdGender,
                command.IdCountry,
                defaultRoleId);

            if (Domain is null)
                return UserResult.FailureResult(ErrorCode.DomainCreationError, "Ошибка создание домен модели.");

            #endregion

            try
            {
                var createdUserDomain = await _userRepository.CreateAsync(Domain);

                if (createdUserDomain is null)
                    return UserResult.FailureResult(ErrorCode.SaveUserError, "Ошибка сохранение пользователя.");

                var userDto = createdUserDomain.ToDto();

                return UserResult.SuccessResult(userDto);
            }
            catch (Exception)
            {
                //TODO: Сделать логирование ошибки
                return UserResult.FailureResult(ErrorCode.UnknownError, "Во время регистрации произошла непредвиденная ошибка");
            }
        }
    }
}
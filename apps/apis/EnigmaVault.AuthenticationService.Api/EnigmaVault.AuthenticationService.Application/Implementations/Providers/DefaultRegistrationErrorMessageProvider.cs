using EnigmaVault.AuthenticationService.Application.Abstractions.Providers;
using EnigmaVault.AuthenticationService.Application.Enums;

namespace EnigmaVault.AuthenticationService.Application.Implementations.Providers
{
    public class DefaultRegistrationErrorMessageProvider : IRegistrationErrorMessageProvider
    {
        public string GetMessage(ErrorCode errorCode)
        {
            return errorCode switch
            {
                ErrorCode.LoginAlreadyTaken => "Этот логин уже занят.",
                ErrorCode.EmailAlreadyRegistered => "Этот email уже зарегистрирован.",
                ErrorCode.PhoneAlreadyRegistered => "Этот телефон уже зарегистрирован.",
                ErrorCode.WeakPassword => "Пароль не соответствует требованиям безопасности.",
                ErrorCode.ValidationFailed => "Одно или несколько полей не прошли валидацию.",
                ErrorCode.DomainCreationError => "Ошибка при создании доменного объекта пользователя.",
                ErrorCode.InvalidRole => "Указана неверная роль.",
                ErrorCode.InvalidAccountStatus => "Указан неверный статус аккаунта.",
                ErrorCode.SaveUserError => "Ошибка при сохранении пользователя в базе данных.",
                ErrorCode.UnknownError or _ => "Произошла непредвиденная ошибка."
            };
        }
    }
}
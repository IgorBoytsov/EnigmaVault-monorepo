using EnigmaVault.AuthenticationService.Application.Abstractions.Providers;
using EnigmaVault.AuthenticationService.Application.Enums;

namespace EnigmaVault.AuthenticationService.Application.Implementations.Providers
{
    public class DefaultAuthenticateErrorMessageProvider : IDefaultErrorMessageProvider
    {
        public string GetMessage(ErrorCode errorCode)
        {
            return errorCode switch
            {
                ErrorCode.LoginNotExist => "Такого логина не существует.",
                ErrorCode.InvalidPassword => "Указан не верный пароль.",
                ErrorCode.ValidationFailed => "Одно или несколько полей не прошли валидацию.",
                ErrorCode.DomainCreationError => "Ошибка при создании доменного объекта пользователя.",
                ErrorCode.UnknownError or _ => "Произошла непредвиденная ошибка."
            };
        }
    }
}
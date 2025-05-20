using EnigmaVault.AuthenticationService.Application.Enums;
using EnigmaVault.AuthenticationService.Application.Implementations.Hashers;
using EnigmaVault.AuthenticationService.Application.Implementations.Providers;

namespace MyApplication.Tests
{
    internal class DefaultErrorMessageProviderTests
    {
        private DefaultRegistrationErrorMessageProvider _registrationErrorMessageProvider;
        private DefaultAuthenticateErrorMessageProvider _authenticateErrorMessageProvider;

        [SetUp]
        public void SetUp()
        {
            _registrationErrorMessageProvider = new DefaultRegistrationErrorMessageProvider();
            _authenticateErrorMessageProvider = new DefaultAuthenticateErrorMessageProvider();
        }

        [Test]
        public void RegistrationErrorMessageProvider_GetMessage_ReturnsStringMessage()
        {
            var loginAlreadyTaken = _registrationErrorMessageProvider.GetMessage(ErrorCode.LoginAlreadyTaken);
            var emailAlreadyRegistered = _registrationErrorMessageProvider.GetMessage(ErrorCode.EmailAlreadyRegistered);
            var phoneAlreadyRegistered = _registrationErrorMessageProvider.GetMessage(ErrorCode.PhoneAlreadyRegistered);
            var weakPassword = _registrationErrorMessageProvider.GetMessage(ErrorCode.WeakPassword);
            var validationFailed = _registrationErrorMessageProvider.GetMessage(ErrorCode.ValidationFailed);
            var domainCreationError = _registrationErrorMessageProvider.GetMessage(ErrorCode.DomainCreationError);
            var invalidRole = _registrationErrorMessageProvider.GetMessage(ErrorCode.InvalidRole);
            var invalidAccountStatus = _registrationErrorMessageProvider.GetMessage(ErrorCode.InvalidAccountStatus);
            var saveUserError = _registrationErrorMessageProvider.GetMessage(ErrorCode.SaveUserError);
            var unknownError = _registrationErrorMessageProvider.GetMessage(ErrorCode.UnknownError);

            Assert.Multiple(() =>
            {
                Assert.That(loginAlreadyTaken, Is.EqualTo("Этот логин уже занят."));
                Assert.That(emailAlreadyRegistered, Is.EqualTo("Этот email уже зарегистрирован."));
                Assert.That(phoneAlreadyRegistered, Is.EqualTo("Этот телефон уже зарегистрирован."));
                Assert.That(weakPassword, Is.EqualTo("Пароль не соответствует требованиям безопасности."));
                Assert.That(validationFailed, Is.EqualTo("Одно или несколько полей не прошли валидацию."));
                Assert.That(domainCreationError, Is.EqualTo("Ошибка при создании объекта пользователя."));
                Assert.That(invalidRole, Is.EqualTo("Указана неверная роль."));
                Assert.That(invalidAccountStatus, Is.EqualTo("Указан неверный статус аккаунта."));
                Assert.That(saveUserError, Is.EqualTo("Ошибка при сохранении пользователя в базе данных."));
                Assert.That(unknownError, Is.EqualTo("Произошла непредвиденная ошибка."));
            });
        }

        [Test]
        public void AuthenticateErrorMessageProvider_GetMessage_ReturnsStringMessage()
        {
            var loginNotExist = _authenticateErrorMessageProvider.GetMessage(ErrorCode.LoginNotExist);
            var invalidPassword = _authenticateErrorMessageProvider.GetMessage(ErrorCode.InvalidPassword);
            var validationFailed = _authenticateErrorMessageProvider.GetMessage(ErrorCode.ValidationFailed);
            var domainCreationError = _authenticateErrorMessageProvider.GetMessage(ErrorCode.DomainCreationError);
            var unknownError = _authenticateErrorMessageProvider.GetMessage(ErrorCode.UnknownError);
           

            Assert.Multiple(() =>
            {
                Assert.That(loginNotExist, Is.EqualTo("Такого логина не существует."));
                Assert.That(invalidPassword, Is.EqualTo("Указан не верный пароль."));
                Assert.That(validationFailed, Is.EqualTo("Одно или несколько полей не прошли валидацию."));
                Assert.That(domainCreationError, Is.EqualTo("Ошибка при создании доменного объекта пользователя."));
                Assert.That(unknownError, Is.EqualTo("Произошла непредвиденная ошибка."));
            });
        }
    }
}
using EnigmaVault.AuthenticationService.Application.Abstractions.Hashers;
using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Application.DTOs.Commands;
using EnigmaVault.AuthenticationService.Application.Enums;
using EnigmaVault.AuthenticationService.Application.Implementations.Hashers;
using EnigmaVault.AuthenticationService.Application.Implementations.UseCases;
using EnigmaVault.AuthenticationService.Domain.DomainModels;
using EnigmaVault.AuthenticationService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace MyApplication.Tests
{
    public class AuthenticateUserUseCaseTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IPasswordHasher> _passwordHasherMock;
        private Mock<ILogger<AuthenticateUserUseCase>> _loggerAuthenticateUserUseCaseMock;
        private Mock<ILogger<Argon2PasswordHasher>> _loggerArgonMock;
        private AuthenticateUserUseCase _useCase;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _loggerAuthenticateUserUseCaseMock = new Mock<ILogger<AuthenticateUserUseCase>>();
            _loggerArgonMock = new Mock<ILogger<Argon2PasswordHasher>>();
            _useCase = new AuthenticateUserUseCase(_userRepositoryMock.Object, _passwordHasherMock.Object, _loggerAuthenticateUserUseCaseMock.Object);
        }

        private static AuthenticateUserCommand CreateSampleAuthenticateUserCommand(string? login = "LightPlay", string password = "ValidPass123.!")
        {
            return new AuthenticateUserCommand()
            {
                Login = login,
                Password = password,
            };
        }

        private UserDomain CreateSampleUserDomain(string? login = "testLogin", string? userName = "LightPlay", string? email = "test@example.com", string? phone = "01234567891", string? password = "4444")
        {
            var errors = new List<string>();
            var passwordHasher = new Argon2PasswordHasher(_loggerArgonMock.Object);

            var loginResult = Login.TryCreate(login, out var loginResultVo);
            var emailAddressResult = EmailAddress.TryCreate(email, out var emailAddressVo);
            var phoneNumberResult = PhoneNumber.TryCreate(phone, out var phoneNumberVo);
            var passwordHash = passwordHasher.HashPassword(password);

            var (User, DomainErrors) = UserDomain.Create(loginResultVo, userName, passwordHash, emailAddressVo, phoneNumberVo, 1, 1, 1, 1);

            if (DomainErrors is not null)
                for (int i = 0; i < loginResult.Errors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from CreateSimpleUserDomain] {DomainErrors[i]}");

            if (!loginResult.IsValid)
                for (int i = 0; i < loginResult.Errors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from Login.TryCreate] {loginResult.Errors[i]}");

            if (!emailAddressResult.IsValid)
                for (int i = 0; i < emailAddressResult.Errors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from EmailAddress.TryCreate] {emailAddressResult.Errors[i]}");

            if (!phoneNumberResult.IsValid)
                for (int i = 0; i < phoneNumberResult.Errors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from PhoneNumber.TryCreate] {phoneNumberResult.Errors[i]}");

            return User;
        }

        [Test]
        public async Task AuthenticateAsync_ValidCommand_ReturnsSuccessResult()
        {
            var userDomain = CreateSampleUserDomain();
            var command = CreateSampleAuthenticateUserCommand();

            _userRepositoryMock.Setup(r => r.ExistsByLoginAsync(It.IsAny<string>())).ReturnsAsync(true);
            _userRepositoryMock.Setup(r => r.GetUserByLoginAsync(It.IsAny<string>())).ReturnsAsync(userDomain);
            _passwordHasherMock.Setup(h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var userResult = await _useCase.AuthenticateAsync(command);

            Assert.That(userResult, Is.Not.Null);
            Assert.That(userResult.Success, Is.True);
        }

        [Test]
        public async Task AuthenticateAsync_InvalidCommand_ReturnsValidationResult_WhenLoginAndPasswordEmpty()
        {
            var command = CreateSampleAuthenticateUserCommand(login: string.Empty, password: string.Empty);

            var userResult = await _useCase.AuthenticateAsync(command);

            if (userResult.ValidationErrors.Any())
                for (int i = 0; i < userResult.ValidationErrors.Count; i++)
                    TestContext.Out.WriteLine($"Ошибка валидации: {userResult.ValidationErrors[i]}");

            Assert.That(userResult, Is.Not.Null);
            Assert.That(userResult.User, Is.Null);
            Assert.That(userResult.Success, Is.False);
            Assert.That(userResult.ValidationErrors, Is.Not.Null);
            Assert.That(userResult.ValidationErrors.Any(), Is.True);
        }

        [Test]
        public async Task AuthenticateAsync_InvalidCommand_ReturnsFailureResult_WhenLoginNotExist()
        {
            var command = CreateSampleAuthenticateUserCommand();

             _userRepositoryMock.Setup(u => u.ExistsByLoginAsync(It.IsAny<string>())).ReturnsAsync(false);

            var userResult = await _useCase.AuthenticateAsync(command);

            TestContext.Out.WriteLine($"Ошибка: {userResult.ErrorMessage}");

            Assert.That(userResult, Is.Not.Null);
            Assert.That(userResult.User, Is.Null);
            Assert.That(userResult.Success, Is.False);
            Assert.That(userResult.ErrorMessage, Is.EqualTo($"Логин {command.Login} не зарегистрирован."));
            Assert.That(userResult.ErrorCode, Is.EqualTo(ErrorCode.LoginNotExist));
            Assert.That(userResult.ValidationErrors.Any(), Is.False);
        }

        [Test]
        public async Task AuthenticateAsync_InvalidValidCommand_ReturnsFailureResult_WhenNotVerifyPassword()
        {
            var userDomain = CreateSampleUserDomain();
            var command = CreateSampleAuthenticateUserCommand();

            _userRepositoryMock.Setup(r => r.ExistsByLoginAsync(It.IsAny<string>())).ReturnsAsync(true);
            _userRepositoryMock.Setup(r => r.GetUserByLoginAsync(It.IsAny<string>())).ReturnsAsync(userDomain);
            _passwordHasherMock.Setup(h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var userResult = await _useCase.AuthenticateAsync(command);

            Assert.That(userResult, Is.Not.Null);
            Assert.That(userResult.User, Is.Null);
            Assert.That(userResult.Success, Is.False);
            Assert.That(userResult.ErrorMessage, Is.EqualTo("Указан не верный пароль."));
            Assert.That(userResult.ErrorCode, Is.EqualTo(ErrorCode.InvalidPassword));
            Assert.That(userResult.ValidationErrors.Any(), Is.False);
        }

        [Test]
        public async Task AuthenticateAsync_UnknownError_ReturnsFailureResult_ThrowException()
        {
            var command = CreateSampleAuthenticateUserCommand();

            _userRepositoryMock.Setup(r => r.ExistsByLoginAsync(It.IsAny<string>())).ReturnsAsync(true);
            _userRepositoryMock.Setup(r => r.GetUserByLoginAsync(It.IsAny<string>())).Throws(new Exception("Исключение"));
            _passwordHasherMock.Setup(h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var userResult = await _useCase.AuthenticateAsync(command);

            Assert.That(userResult, Is.Not.Null);
            Assert.That(userResult.User, Is.Null);
            Assert.That(userResult.Success, Is.False);
            Assert.That(userResult.ErrorMessage, Is.EqualTo("Во время аутентификации произошла непредвиденная ошибка"));
            Assert.That(userResult.ErrorCode, Is.EqualTo(ErrorCode.UnknownError));
            Assert.That(userResult.ValidationErrors.Any(), Is.False);
        }

        [Test]
        public void AuthenticateAsync_Exception_ThrowArgumentNullExceptionWhenCommandIsNul()
        {
            AuthenticateUserCommand command = null!;

            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _useCase.AuthenticateAsync(command));

            Assert.That(ex.ParamName, Is.EqualTo($"Значение {nameof(command)} было пустым."));
        }
    }
}
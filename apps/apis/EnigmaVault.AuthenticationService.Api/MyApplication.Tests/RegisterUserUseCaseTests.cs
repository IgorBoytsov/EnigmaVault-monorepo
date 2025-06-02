using Castle.Core.Logging;
using EnigmaVault.AuthenticationService.Application.Abstractions.Hashers;
using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Application.DTOs.Commands;
using EnigmaVault.AuthenticationService.Application.Enums;
using EnigmaVault.AuthenticationService.Application.Implementations.UseCases;
using EnigmaVault.AuthenticationService.Domain.DomainModels;
using EnigmaVault.AuthenticationService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace MyApplication.Tests
{
    [TestFixture]
    public class RegisterUserUseCaseTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IPasswordHasher> _passwordHasherMock;
        private Mock<ILogger<RegisterUserUseCase>> _loggerMock;
        private RegisterUserUseCase _useCase;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _loggerMock = new Mock<ILogger<RegisterUserUseCase>>();
            _useCase = new RegisterUserUseCase(_userRepositoryMock.Object, _passwordHasherMock.Object, _loggerMock.Object);
        }

        private static RegisterUserCommand CreateSampleRegisterUserCommand(string? login = "LightPlay", string? userName = "Игорь", string password = "ValidPass123.!", string? email = "ValidEmail@yandex.ru", string? phone = "+7004001010")
        {
            return new RegisterUserCommand()
            {
                Login = login,
                UserName = userName,
                Password = password,
                Email = email,
                Phone = phone,
                IdGender = 1,
                IdCountry = 1,
            }; ;
        }

        [Test]
        public async Task RegisterASync_ValidCommand_ReturnsSuccessResult()
        {
            var command = CreateSampleRegisterUserCommand();

            _userRepositoryMock.Setup(r => r.ExistsByLoginAsync(It.IsAny<string>())).ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.ExistsByPhoneAsync(It.IsAny<string>())).ReturnsAsync(false);
            _passwordHasherMock.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hashedPassword");

            var loginResult = Login.TryCreate(command.Login, out var loginResultVo);
            var emailAddressResult = EmailAddress.TryCreate(command.Email, out var emailAddressVo);
            var phoneNumberResult = PhoneNumber.TryCreate(command.Phone, out var phoneNumberVo);

            var userDomain = UserDomain.Create(loginResultVo, command.UserName, "hashedPassword", emailAddressVo, phoneNumberVo, 1, 1, 1, 1).User;

            _userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<UserDomain>())).ReturnsAsync(userDomain);

            var result = await _useCase.RegisterAsync(command);

            if (result.ValidationErrors.Any())
                for (int i = 0; i < result.ValidationErrors.Count; i++)
                    TestContext.Out.WriteLine(result.ValidationErrors[i]);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True, "Регистрация прошла успешно.");
                Assert.That(result.User, Is.Not.Null, "Пользователь не должен быть null");
                Assert.That(result.ErrorCode, Is.Null, "Код ошибки должен быть null");
                Assert.That(result.ErrorMessage, Is.Null, "Сообщение об ошибки должно быть null");
                Assert.That(result.ValidationErrors, Is.Empty.Or.Null, "Список ошибок валидиаций должен быть пустым либо иметь null ссылку");
            });
        }

        [Test]
        public async Task RegisterAsync_ReturnsSuccessResult_WhenNullPhoneInCommand()
        {
            var command = CreateSampleRegisterUserCommand(phone: " ");

            _userRepositoryMock.Setup(r => r.ExistsByLoginAsync(It.IsAny<string>())).ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>())).ReturnsAsync(false);
            _userRepositoryMock.Setup(r => r.ExistsByPhoneAsync(It.IsAny<string>())).ReturnsAsync(false);
            _passwordHasherMock.Setup(h => h.HashPassword(It.IsAny<string>())).Returns("hashedPassword");

            var loginResult = Login.TryCreate(command.Login, out var loginResultVo);
            var emailAddressResult = EmailAddress.TryCreate(command.Email, out var emailAddressVo);
            var phoneNumberResult = PhoneNumber.TryCreate(command.Phone, out var phoneNumberVo);

            var userDomain = UserDomain.Create(loginResultVo, command.UserName, "hashedPassword", emailAddressVo, phoneNumberVo, 1, 1, 1, 1).User;

            _userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<UserDomain>())).ReturnsAsync(userDomain);


            var result = await _useCase.RegisterAsync(command);

            if (result.ValidationErrors.Any())
                for (int i = 0; i < result.ValidationErrors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from RegisterUserUseCase] - Ошибка: {result.ValidationErrors[i]}");

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True, "Регистрация прошла успешно.");
                Assert.That(result.User, Is.Not.Null, "Пользователь не должен быть null");
                Assert.That(result.ErrorCode, Is.Null, "Код ошибки должен быть null");
                Assert.That(result.ErrorMessage, Is.Null, "Сообщение об ошибки должно быть null");
                Assert.That(result.ValidationErrors, Is.Empty.Or.Null, "Список ошибок валидиаций должен быть пустым либо иметь null ссылку");
            });
        }

        [Test]
        public async Task RegisterAsync_ReturnsValidationFailure_WhenInCorrectLoginInCommand()
        {
            var command = CreateSampleRegisterUserCommand(login: "Не валидный логин");

            var result = await _useCase.RegisterAsync(command);

            if (result.ValidationErrors.Any())
                for (int i = 0; i < result.ValidationErrors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from RegisterUserUseCase] - Ошибка: {result.ValidationErrors[i]}");

            Assert.Multiple(() =>
            { 
                Assert.That(result.Success, Is.False, "Регистрация прошла c ошибкой занятого логина.");
                Assert.That(result.User, Is.Null, "Пользователь должен быть null");
                Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.ValidationFailed), "Код ошибки должен быть не null");
                Assert.That(result.ErrorMessage, Is.Not.Null, "Сообщение об ошибки не должно быть null");
                Assert.That(result.ValidationErrors, Is.Not.Null, "Список ошибок валидиаций не должен быть пустым либо иметь ссылку");
            });
        }

        [Test]
        public async Task RegisterAsync_ReturnsValidationFailure_WhenInCorrectEmailInCommand()
        {
            var command = CreateSampleRegisterUserCommand(email: "invalid.Email!2.yandex.ru");

            var result = await _useCase.RegisterAsync(command);

            if (result.ValidationErrors.Any())
                for (int i = 0; i < result.ValidationErrors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from RegisterUserUseCase] - Ошибка: {result.ValidationErrors[i]}");

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False, "Регистрация прошла c ошибкой занятого логина.");
                Assert.That(result.User, Is.Null, "Пользователь должен быть null");
                Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.ValidationFailed), "Код ошибки должен быть не null");
                Assert.That(result.ErrorMessage, Is.Not.Null, "Сообщение об ошибки не должно быть null");
                Assert.That(result.ValidationErrors, Is.Not.Null, "Список ошибок валидиаций не должен быть пустым либо иметь ссылку");
            });
        }

        [Test]
        public async Task RegisterAsync_ReturnsValidationFailure_WhenInCorrectAllValuesInCommand()
        {
            var command = CreateSampleRegisterUserCommand(login: "Не валидный логин", password: " ", userName: " ", email: "invalid.Email!2.yandex.ru", phone: " ");

            var result = await _useCase.RegisterAsync(command);

            if (result.ValidationErrors.Any())
                for (int i = 0; i < result.ValidationErrors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from RegisterUserUseCase] - Ошибка: {result.ValidationErrors[i]}");

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False, "Регистрация прошла c ошибкой занятого логина.");
                Assert.That(result.User, Is.Null, "Пользователь должен быть null");
                Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.ValidationFailed), "Код ошибки должен быть не null");
                Assert.That(result.ErrorMessage, Is.Not.Null, "Сообщение об ошибки не должно быть null");
                Assert.That(result.ValidationErrors, Is.Not.Null, "Список ошибок валидиаций не должен быть пустым либо иметь ссылку");
            });
        }

        [Test]
        public async Task RegisterAsync_ReturnsValidationFailure_WhenPasswordIsNullOrEmptyInCommand()
        {
            var command = CreateSampleRegisterUserCommand(password: " ");

            var result = await _useCase.RegisterAsync(command);

            if (result.ValidationErrors.Any())
                for (int i = 0; i < result.ValidationErrors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from RegisterUserUseCase] - Ошибка: {result.ValidationErrors[i]}");

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False, "Регистрация прошла c ошибкой занятого логина.");
                Assert.That(result.User, Is.Null, "Пользователь должен быть null");
                Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.ValidationFailed), "Код ошибки должен быть не null");
                Assert.That(result.ErrorMessage, Is.Not.Null, "Сообщение об ошибки не должно быть null");
                Assert.That(result.ValidationErrors, Is.Not.Null, "Список ошибок валидиаций не должен быть пустым либо иметь ссылку");
            });
        }

        [Test]
        public async Task RegisterAsync_DuplicateLogin_ReturnsFailureResult()
        {
            var command = CreateSampleRegisterUserCommand();

            _userRepositoryMock.Setup(r => r.ExistsByLoginAsync(It.IsAny<string>())).ReturnsAsync(true);

            var result = await _useCase.RegisterAsync(command);

            if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                TestContext.Out.WriteLine(result.ErrorMessage);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False, "Регистрация прошла с ошибками.");
                Assert.That(result.User, Is.Null, "Пользователь должен быть null");
                Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.LoginAlreadyTaken), "Код ошибки не должен быть null");
                Assert.That(result.ErrorMessage, Is.Not.Null.Or.Not.Empty, "Сообщение об ошибки не должно быть null либо пустым.");
            });
        }

        [Test]
        public async Task RegisterAsync_DuplicateEmail_ReturnsFailureResult()
        {
            var command = CreateSampleRegisterUserCommand();

            _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>())).ReturnsAsync(true);

            var result = await _useCase.RegisterAsync(command);

            if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                TestContext.Out.WriteLine(result.ErrorMessage);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False, "Регистрация прошла с ошибками.");
                Assert.That(result.User, Is.Null, "Пользователь должен быть null");
                Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.EmailAlreadyRegistered), "Код ошибки не должен быть null");
                Assert.That(result.ErrorMessage, Is.Not.Null.Or.Not.Empty, "Сообщение об ошибки не должно быть null либо пустым.");
            });
        }

        [Test]
        public async Task RegisterAsync_DuplicatePhone_ReturnsFailureResult()
        {
            var command = CreateSampleRegisterUserCommand();

            _userRepositoryMock.Setup(r => r.ExistsByPhoneAsync(It.IsAny<string>())).ReturnsAsync(true);

            var result = await _useCase.RegisterAsync(command);

            if (!string.IsNullOrWhiteSpace(result.ErrorMessage))
                TestContext.Out.WriteLine(result.ErrorMessage);

            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False, "Регистрация прошла с ошибками.");
                Assert.That(result.User, Is.Null, "Пользователь должен быть null");
                Assert.That(result.ErrorCode, Is.EqualTo(ErrorCode.PhoneAlreadyRegistered), "Код ошибки не должен быть null");
                Assert.That(result.ErrorMessage, Is.Not.Null.Or.Not.Empty, "Сообщение об ошибки не должно быть null либо пустым.");
            });
        }
    }
}
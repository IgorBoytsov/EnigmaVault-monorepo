using EnigmaVault.AuthenticationService.Application.Abstractions.Hashers;
using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Application.DTOs.Commands;
using EnigmaVault.AuthenticationService.Application.Enums;
using EnigmaVault.AuthenticationService.Application.Implementations.UseCases;
using Moq;

namespace MyApplication.Tests
{
    public class RecoveryAccessUserUseCaseTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IPasswordHasher> _passwordHasherMock;
        private RecoveryAccessUserUseCase _useCase;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _useCase = new RecoveryAccessUserUseCase(_userRepositoryMock.Object, _passwordHasherMock.Object);
        }

        private static RecoveryAccessUserCommand CreateSampleRecoveryAccessUserCommand(string? login = "LightPlay", string email = "test@example.com", string password = "ValidPass123.!")
        {
            return new RecoveryAccessUserCommand()
            {
                Login = login,
                Email = email,
                NewPassword = password
            };
        }

        [Test]
        public async Task RecoveryAccess_ValidCommand_ReturnsSuccessResult()
        {
            var command = CreateSampleRecoveryAccessUserCommand();

            _userRepositoryMock.Setup(r => r.ExistsByLoginAsync(It.IsAny<string>())).ReturnsAsync(true);
            _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>())).ReturnsAsync(true);
            _userRepositoryMock.Setup(r => r.UpdatePasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            _passwordHasherMock.Setup(h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var userResult = await _useCase.RecoveryAccessAsync(command);

            Assert.That(userResult, Is.Not.Null);
            Assert.That(userResult.Success, Is.True);
        }

        [Test]
        public async Task RecoveryAccess_ValidCommand_ReturnsFailureResult()
        {
            var command = CreateSampleRecoveryAccessUserCommand();

            _userRepositoryMock.Setup(r => r.ExistsByLoginAsync(It.IsAny<string>())).ReturnsAsync(true);
            _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>())).ReturnsAsync(true);
            _userRepositoryMock.Setup(r => r.UpdatePasswordAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
            _passwordHasherMock.Setup(h => h.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var userResult = await _useCase.RecoveryAccessAsync(command);

            Assert.That(userResult, Is.Not.Null);
            Assert.That(userResult.Success, Is.False);
            Assert.That(userResult.ErrorMessage, Is.EqualTo("Не удалось сохранить пароль."));
            Assert.That(userResult.ErrorCode, Is.EqualTo(ErrorCode.SaveUserError));
        }

        [Test]
        public void RecoveryAccess_InvalidCommand_ThrowsArgumentNullException()
        {
            RecoveryAccessUserCommand command = null;

            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _useCase.RecoveryAccessAsync(command));

            TestContext.Out.WriteLine(nameof(command));

            Assert.That(ex, Is.Not.Null);
            Assert.That(ex.ParamName, Is.EqualTo(nameof(command)));
        }

        [Test]
        public async Task RecoveryAccess_InvalidCommand_ReturnsValidationFailureResult_WhenLoginAndEmailAndPasswordEmpty()
        {
            var command = CreateSampleRecoveryAccessUserCommand(login: "", email: "", password: "");

            var userResult = await _useCase.RecoveryAccessAsync(command);

            foreach (var item in userResult.ValidationErrors)
                TestContext.Out.WriteLine(item);

            Assert.That(userResult, Is.Not.Null);
            Assert.That(userResult.Success, Is.False);
            Assert.That(userResult.ValidationErrors.Any(), Is.True);
            Assert.That(userResult.ValidationErrors, Contains.Item("Вы не указали логин."));
            Assert.That(userResult.ValidationErrors, Contains.Item("Вы не указали почту."));
            Assert.That(userResult.ValidationErrors, Contains.Item("Вы не указали пароль."));
            Assert.That(userResult.ErrorMessage, Is.EqualTo(null));
            Assert.That(userResult.ErrorCode, Is.EqualTo(ErrorCode.ValidationFailed));
        }

        [Test]
        public async Task RecoveryAccess_InvalidCommand_ReturnsValidationFailureResult_WhenPasswordValueObjectReturnedNull()
        {
            var command = CreateSampleRecoveryAccessUserCommand(password: "efe");

            var userResult = await _useCase.RecoveryAccessAsync(command);

            foreach (var item in userResult.ValidationErrors)
                TestContext.Out.WriteLine(item);

            Assert.That(userResult, Is.Not.Null);
            Assert.That(userResult.Success, Is.False);
            Assert.That(userResult.ValidationErrors.Any(), Is.True);
            Assert.That(userResult.ValidationErrors, Contains.Item("В пароле должна быть как минимум 1 буква верхнего регистра."));
            Assert.That(userResult.ValidationErrors, Contains.Item("В пароле должна быть как минимум 1 цифра."));
            Assert.That(userResult.ValidationErrors, Contains.Item("В пароле должен быть как минимум 1 символ."));
            Assert.That(userResult.ErrorMessage, Is.EqualTo(null));
            Assert.That(userResult.ErrorCode, Is.EqualTo(ErrorCode.ValidationFailed));
        }

        [Test]
        public async Task RecoveryAccess_ValidCommand_ReturnsFailureResult_WhenLoginNotExist()
        {
            var command = CreateSampleRecoveryAccessUserCommand();

            _userRepositoryMock.Setup(r => r.ExistsByLoginAsync(It.IsAny<string>())).ReturnsAsync(false);

            var userResult = await _useCase.RecoveryAccessAsync(command);

            Assert.That(userResult, Is.Not.Null);
            Assert.That(userResult.Success, Is.False);
            Assert.That(userResult.ErrorCode, Is.EqualTo(ErrorCode.LoginNotExist));
        }

        [Test]
        public async Task RecoveryAccess_ValidCommand_ReturnsFailureResult_WhenEmailNotExist()
        {
            var command = CreateSampleRecoveryAccessUserCommand();

            _userRepositoryMock.Setup(r => r.ExistsByLoginAsync(It.IsAny<string>())).ReturnsAsync(true);
            _userRepositoryMock.Setup(r => r.ExistsByEmailAsync(It.IsAny<string>())).ReturnsAsync(false);

            var userResult = await _useCase.RecoveryAccessAsync(command);

            Assert.That(userResult, Is.Not.Null);
            Assert.That(userResult.Success, Is.False);
            Assert.That(userResult.ErrorCode, Is.EqualTo(ErrorCode.EmailNotExist));
        }
    }
}
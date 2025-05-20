using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Application.Implementations.Hashers;
using EnigmaVault.AuthenticationService.Domain.DomainModels;
using EnigmaVault.AuthenticationService.Domain.ValueObjects;
using EnigmaVault.AuthenticationService.Infrastructure.Data;
using EnigmaVault.AuthenticationService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework.Internal;

namespace MyInfrastructure.Tests
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly UsersDBContext _context;
        private readonly IUserRepository _userRepository;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<UsersDBContext>().UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

            _context = new UsersDBContext(options);
            _context.Database.EnsureCreated();

            _userRepository = new UserRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private UserDomain CreateSampleUserDomain(string? login = "testLogin", string? userName = "LightPlay", string? email = "test@example.com", string? phone = "01234567891", string? password = "4444")
        {
            var errors = new List<string>();
            var passwordHasher = new Argon2PasswordHasher();

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

        /*--Create----------------------------------------------------------------------------------------*/

        [Test]
        public async Task CreateAsync_ShouldAddUserToDatabase_AndReturnAddedDomain()
        {
            var userDomainToCreate = CreateSampleUserDomain();

            TestContext.Out.WriteLine($"[SETUP INFO from CreateSampleUserDomain] - Исходные данные");
            TestContext.Out.WriteLine($"[SETUP INFO from CreateSampleUserDomain] - Логин: {userDomainToCreate.Login}");
            TestContext.Out.WriteLine($"[SETUP INFO from CreateSampleUserDomain] - Имя пользователя: {userDomainToCreate.UserName}");
            TestContext.Out.WriteLine($"[SETUP INFO from CreateSampleUserDomain] - Емаил: {userDomainToCreate.Email}");
            TestContext.Out.WriteLine($"[SETUP INFO from CreateSampleUserDomain] - Телефон: {userDomainToCreate.Phone}");
            TestContext.Out.WriteLine($"[SETUP INFO from CreateSampleUserDomain] - Хеш: {userDomainToCreate.PasswordHash}\n");

            var returnedUserDomain = await _userRepository.CreateAsync(userDomainToCreate);

            //Assert.That(returnedUserDomain, Is.SameAs(userDomainToCreate));

            Assert.Multiple(() =>
            {
                Assert.That(returnedUserDomain!.Login, Is.EqualTo(userDomainToCreate.Login));
                Assert.That(returnedUserDomain.UserName, Is.EqualTo(userDomainToCreate.UserName));
                Assert.That(returnedUserDomain.Email, Is.EqualTo(userDomainToCreate.Email));
                Assert.That(returnedUserDomain.Phone, Is.EqualTo(userDomainToCreate.Phone));
                Assert.That(returnedUserDomain.PasswordHash, Is.EqualTo(userDomainToCreate.PasswordHash));
            });

            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Login == userDomainToCreate.Login);
            Assert.That(userInDb, Is.Not.Null);

            TestContext.Out.WriteLine($"[SETUP INFO from FirstOrDefaultAsync] - Возвращаемые данные");
            TestContext.Out.WriteLine($"[SETUP INFO from FirstOrDefaultAsync] - Логин: {userInDb.Login}");
            TestContext.Out.WriteLine($"[SETUP INFO from FirstOrDefaultAsync] - Имя пользователя: {userInDb.UserName}");
            TestContext.Out.WriteLine($"[SETUP INFO from FirstOrDefaultAsync] - Емаил: {userInDb.Email}");
            TestContext.Out.WriteLine($"[SETUP INFO from FirstOrDefaultAsync] - Телефон: {userInDb.Phone}");
            TestContext.Out.WriteLine($"[SETUP INFO from FirstOrDefaultAsync] - Хеш: {userInDb.PasswordHash}\n");

            Assert.Multiple(() =>
            {
                Assert.That(userInDb.Login, Is.EqualTo(userDomainToCreate.Login));
                Assert.That(userInDb.Email, Is.EqualTo(userDomainToCreate.Email));
                Assert.That(userInDb.Phone, Is.EqualTo(userDomainToCreate.Phone));
                Assert.That(userInDb.UserName, Is.EqualTo(userDomainToCreate.UserName));
                Assert.That(userInDb.PasswordHash, Is.EqualTo(userDomainToCreate.PasswordHash));
            });

            Assert.That(userInDb.IdUser, Is.GreaterThan(0));
        }

        /*--Get-------------------------------------------------------------------------------------------*/

        [Test]
        public async Task GetHashByLoginAsync_ShouldFindHash_AndReturnTheExactStoredHash() 
        {
            string plainPassword = "StrongPassword11!!"; 
            string login = "SecondLogin"; 

            var user = CreateSampleUserDomain(login: login, password: plainPassword);
            string expectedHash = user.PasswordHash;

            await _userRepository.CreateAsync(user);

            var actualStoredHash = await _userRepository.GetHashByLoginAsync(user.Login);

            TestContext.Out.WriteLine($"Ожидаемый хеш: {expectedHash}");
            TestContext.Out.WriteLine($"Хэш из БД: {actualStoredHash}");

            Assert.That(actualStoredHash, Is.EqualTo(expectedHash));
        }

        [Test]
        public void GetHashByLoginAsync_ShouldFindHash_AndNotReturnTheExactStoredHash_AndThrowsArgumentNullException()
        {
            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await _userRepository.GetHashByLoginAsync("login"));

            Assert.That(ex.ParamName, Is.EqualTo("Пользователь не найден."));
        }

        /*--Login-----------------------------------------------------------------------------------------*/

        [Test]
        public async Task ExistsByLoginAsync_WhenUserExists_ShouldReturnTrue()
        {
            var testLogin = "existsLogin";
            var existingUser = CreateSampleUserDomain(login: testLogin);
            await _userRepository.CreateAsync(existingUser);

            var result = await _userRepository.ExistsByLoginAsync(testLogin);
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ExistsByLoginAsync_WhenUserExists_ShouldReturnFalse()
        {
            var noExistLogin = "NoExistsLogin";

            var result = await _userRepository.ExistsByLoginAsync(noExistLogin);
            Assert.That(result, Is.False);
        }

        /*--Email-----------------------------------------------------------------------------------------*/

        [Test]
        public async Task ExistsByEmailAsync_WhenUserExists_ShouldReturnTrue()
        {
            var testEmail = "test.mail@yandex.ru";
            var existingUser = CreateSampleUserDomain(email: testEmail);
            await _userRepository.CreateAsync(existingUser);

            var result = await _userRepository.ExistsByEmailAsync(testEmail);
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ExistsByEmailAsync_WhenUserExists_ShouldReturnFalse()
        {
            var testEmail = "testError_mail@yandex.ru";
            var result = await _userRepository.ExistsByEmailAsync(testEmail);
            Assert.That(result, Is.False);
        }

        /*--Phone-----------------------------------------------------------------------------------------*/

        [Test]
        public async Task ExistsByPhoneAsync_WhenUserExists_ShouldReturnTrue()
        {
            var phoneNumberResult = PhoneNumber.TryCreate("+7 123 144 15 90", out var phoneNumberVo);

            var existingUser = CreateSampleUserDomain(phone: phoneNumberVo.Value);
            await _userRepository.CreateAsync(existingUser);

            var result = await _userRepository.ExistsByPhoneAsync(phoneNumberVo.Value);
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ExistsByPhoneAsync_WhenUserExists_ShouldReturnFalse()
        {
            var phoneNumberResult = PhoneNumber.TryCreate("+7 100 100 10 90", out var phoneNumberVo);
            var result = await _userRepository.ExistsByPhoneAsync(phoneNumberVo.Value);
            Assert.That(result, Is.False);
        }
    }
}
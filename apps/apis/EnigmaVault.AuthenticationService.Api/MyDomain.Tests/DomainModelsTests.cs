using EnigmaVault.AuthenticationService.Application.Implementations.Hashers;
using EnigmaVault.AuthenticationService.Domain.DomainModels;
using EnigmaVault.AuthenticationService.Domain.ValueObjects;

namespace MyDomain.Tests
{
    internal class DomainModelsTests
    {
        [Test]
        public void UserDomain_Create_WhenUserCreated()
        {
            string? login = "LightPlay";
            string? userName = "Igor";
            string? email = "test@example.com";
            string? phone = "+7 915 100 10 10";
            string? password = "VeryHardPassword";
            var passwordHasher = new Argon2PasswordHasher();

            Login.TryCreate(login, out var loginResultVo);
            EmailAddress.TryCreate(email, out var emailAddressVo);
            PhoneNumber.TryCreate(phone, out var phoneNumberVo);
            var passwordHash = passwordHasher.HashPassword(password);

            var (User, Message) = UserDomain.Create(0, loginResultVo!, userName, passwordHash, emailAddressVo!, phoneNumberVo, 1, 1, 1, 1);

            Assert.That(User, Is.Not.Null);
        }

        [Test]
        public void UserDomain_Create_WhenUserNotCreated()
        {
            string? login = "LightPlay";
            string? userName = "Igor";
            string? email = "test№example.com"; //Не правильный адрес почты
            string? phone = "8 915 сто 1- 10"; //Не правильный номер
            string? password = "VeryHardPassword";
            var passwordHasher = new Argon2PasswordHasher();

            Login.TryCreate(login, out Login? loginResultVo);
            EmailAddress.TryCreate(email, out EmailAddress? emailAddressVo);
            PhoneNumber.TryCreate(phone, out PhoneNumber? phoneNumberVo);
            var passwordHash = passwordHasher.HashPassword(password);

            var (User, Message) = UserDomain.Create(0, loginResultVo, userName, passwordHash, emailAddressVo, phoneNumberVo, 1, 1, 1, 1);

            Assert.That(User, Is.Null);
        }
    }
}
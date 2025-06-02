using EnigmaVault.AuthenticationService.Application.Implementations.Hashers;
using EnigmaVault.AuthenticationService.Domain.DomainModels;
using EnigmaVault.AuthenticationService.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace MyDomain.Tests
{
    internal class DomainModelsTests
    {
        private Mock<ILogger<Argon2PasswordHasher>> _loggerArgonMock;

        [SetUp]
        public void SetUp()
        {
            _loggerArgonMock = new Mock<ILogger<Argon2PasswordHasher>>();
        }

        /*--UserDomain------------------------------------------------------------------------------------*/

        [Test]
        public void UserDomain_Create_WhenUserCreated()
        {

            string? login = "LightPlay";
            string? userName = "Igor";
            string? email = "test@example.com";
            string? phone = "+7 915 100 10 10";
            string? password = "VeryHardPassword";
            var passwordHasher = new Argon2PasswordHasher(_loggerArgonMock.Object);

            Login.TryCreate(login, out var loginResultVo);
            EmailAddress.TryCreate(email, out var emailAddressVo);
            PhoneNumber.TryCreate(phone, out var phoneNumberVo);
            var passwordHash = passwordHasher.HashPassword(password);

            var (User, Message) = UserDomain.Create(loginResultVo!, userName, passwordHash, emailAddressVo!, phoneNumberVo, 1, 1, 1, 1);

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
            var passwordHasher = new Argon2PasswordHasher(_loggerArgonMock.Object);

            Login.TryCreate(login, out Login? loginResultVo);
            EmailAddress.TryCreate(email, out EmailAddress? emailAddressVo);
            PhoneNumber.TryCreate(phone, out PhoneNumber? phoneNumberVo);
            var passwordHash = passwordHasher.HashPassword(password);

            var (User, Message) = UserDomain.Create(loginResultVo, userName, passwordHash, emailAddressVo, phoneNumberVo, 1, 1, 1, 1);

            Assert.That(User, Is.Null);
        }

        /*--CountryDomain---------------------------------------------------------------------------------*/

        [Test]
        public void CountryDomain_Create_WhenCountryCreated()
        {
            string countryName = "Россия";

            var (Country, Errors) = CountryDomain.Create(countryName);

            if (Errors is not null)
            {
                foreach (var item in Errors)
                    TestContext.Out.WriteLine(item);
            }

            Assert.That(Country, Is.Not.Null);
            Assert.That(Errors, Is.Null);
        }

        [Test]
        public void CountryDomain_Create_WhenCountryNotCreated_BecauseCountryNameIsEmpty()
        {
            string countryName = " ";

            var (Country, Errors) = CountryDomain.Create(countryName);

            if (Errors is not null)
            {
                foreach (var item in Errors)
                    TestContext.Out.WriteLine(item);
            }

            Assert.That(Country, Is.Null);
            Assert.That(Errors.Count, Is.EqualTo(2));
        }

        [Test]
        public void CountryDomain_Create_WhenCountryNotCreated_BecauseCountryLengthLessThree()
        {
            string countryName = "Да";

            var (Country, Errors) = CountryDomain.Create(countryName);

            if (Errors is not null)
            {
                foreach (var item in Errors)
                    TestContext.Out.WriteLine(item);
            }

            Assert.That(Country, Is.Null);
            Assert.That(Errors.Count, Is.EqualTo(1));
        }

        [Test]
        public void CountryDomain_Create_WhenCountryNotCreated_BecauseCountryLengthMoreOneHundred()
        {
            string countryName = "rgergethrtjryjtyiktrthzdhgdzffhfgjtyujkfhdhdhdfthryjtryjtjtyjtyjefergferrgergethrtjryjtyiktrthzdhgdzffhfgjtyujkfhdhdhdfthryjtryjtjtyjtyjefergfer";

            var (Country, Errors) = CountryDomain.Create(countryName);

            if (Errors is not null)
            {
                foreach (var item in Errors)
                    TestContext.Out.WriteLine(item);
            }

            Assert.That(Country, Is.Null);
            Assert.That(Errors.Count, Is.EqualTo(1));
        }

        /*--GenderDomain---------------------------------------------------------------------------------*/

        [Test]
        public void GenderDomain_Create_WhenCountryCreated()
        {
            int genderId = 1;
            string genderName = "Россия";

            var gender = GenderDomain.Reconstitute(genderId, genderName);

            Assert.That(gender, Is.TypeOf<GenderDomain>());
            Assert.That(gender, Is.Not.Null);
            Assert.That(gender.IdGender, Is.EqualTo(genderId));
            Assert.That(gender.GenderName, Is.EqualTo(genderName));
        }
    }
}
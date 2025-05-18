using EnigmaVault.AuthenticationService.Domain.ValueObjects;

namespace MyDomain.Tests
{
    internal class ValueObjectsTests
    {
        #region Email

        [Test]
        public void EmailAddress_TryCreate_WhenEmailAddressCreated()
        {
            string email = "test.email@yandex.ru";
            var emailResult = EmailAddress.TryCreate(email, out EmailAddress? emailVo);

            Assert.That(emailVo, Is.Not.Null);
        }

        [Test]
        public void EmailAddress_TryCreate_WhenEmailAddressNotCreated_BecauseEmailHasTheWrongFormat()
        {
            string email = "test.email№№yandex!ru";
            TestContext.Out.WriteLine($"[SETUP INFO from EmailAddress.TryCreate]: Введен адрес {email}.");
            var emailResult = EmailAddress.TryCreate(email, out EmailAddress? emailVo);

            if (!emailResult.IsValid)
                for (int i = 0; i < emailResult.Errors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from EmailAddress.TryCreate] {emailResult.Errors[i]}");

            Assert.That(emailVo, Is.Null);
        }

        [Test]
        public void EmailAddress_TryCreate_WhenEmailAddressNotCreated_BecauseEmailIsEmpty()
        {
            string email = " ";
            TestContext.Out.WriteLine($"[SETUP INFO from EmailAddress.TryCreate]: Введен адрес {email}.");
            var emailResult = EmailAddress.TryCreate(email, out EmailAddress? emailVo);

            if (!emailResult.IsValid)
                for (int i = 0; i < emailResult.Errors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from EmailAddress.TryCreate] {emailResult.Errors[i]}");

            Assert.That(emailVo, Is.Null);
        }

        [Test]
        public void EmailAddress_TryCreate_WhenEmailAddressNotCreated_BecauseEmailIsNull()
        {
            string? email = null;
            TestContext.Out.WriteLine($"[SETUP INFO from EmailAddress.TryCreate]: Введен адрес {email ?? "null"}.");
            var emailResult = EmailAddress.TryCreate(email, out EmailAddress? emailVo);

            if (!emailResult.IsValid)
                for (int i = 0; i < emailResult.Errors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from EmailAddress.TryCreate] {emailResult.Errors[i]}");

            Assert.That(emailVo, Is.Null);
        }

        #endregion

        #region Login

        [Test]
        public void Login_TryCreate_WhenLoginCreated()
        {
            string login = "LightPlay";
            TestContext.Out.WriteLine($"[SETUP INFO from Login.TryCreate]: Введен логин {login}.");
            var loginResult = Login.TryCreate(login, out Login? loginVo);

            Assert.That(loginVo, Is.Not.Null);
        }

        [Test]
        public void Login_TryCreate_WhenLoginNotCreated_BecauseEmailIsEmpty()
        {
            string login = " ";
            TestContext.Out.WriteLine($"[SETUP INFO from Login.TryCreate]: Введен логин {login}.");
            var loginResult = Login.TryCreate(login, out Login? loginVo);

            if (!loginResult.IsValid)
                for (int i = 0; i < loginResult.Errors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from Login.TryCreate] {loginResult.Errors[i]}");

            Assert.That(loginVo, Is.Null);
        }

        [Test]
        public void Login_TryCreate_WhenLoginNotCreated_BecauseEmailIsNull()
        {
            string? login = null;
            TestContext.Out.WriteLine($"[SETUP INFO from Login.TryCreate]: Введен логин {login ?? "null"}.");
            var loginResult = Login.TryCreate(login, out Login? loginVo);

            if (!loginResult.IsValid)
                for (int i = 0; i < loginResult.Errors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from Login.TryCreate] {loginResult.Errors[i]}");

            Assert.That(loginVo, Is.Null);
        }

        #endregion

        #region Password

        [Test]
        public void Password_TryCreate_WhenPasswordCreated()
        {
            string password = "Very_Strong!PasswordWith12345Digit";
            TestContext.Out.WriteLine($"[SETUP INFO from Password.TryCreate]: Введен пароль {password}.");
            var passwordResult = Password.TryCreate(password, out Password? passwordVo);

            Assert.That(passwordVo, Is.Not.Null);
        }

        [Test]
        public void Password_TryCreate_WhenPasswordNotCreated_BecauseItDoesNotMeetAnyOfTheConditions()
        {
            string password = "👩";
            TestContext.Out.WriteLine($"[SETUP INFO from Password.TryCreate]: Введен пароль {password}.");
            var passwordResult = Password.TryCreate(password, out Password? passwordVo);

            if (!passwordResult.IsValid)
                for (int i = 0; i < passwordResult.Errors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from Password.TryCreate] - Ошибка: {passwordResult.Errors[i]}.");

            Assert.That(passwordVo, Is.Null);
        }

        [Test]
        public void Password_TryCreate_WhenPasswordNotCreated_BecausePasswordIsEmpty()
        {
            string password = " ";
            TestContext.Out.WriteLine($"[SETUP INFO from Password.TryCreate]: Введен пароль {password}.");
            var passwordResult = Password.TryCreate(password, out Password? passwordVo);

            if (!passwordResult.IsValid)
                for(int i = 0; i < passwordResult.Errors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from Password.TryCreate] - Ошибка: {passwordResult.Errors[i]}.");

            Assert.That(passwordVo, Is.Null);
        }

        [Test]
        public void Password_TryCreate_WhenPasswordNotCreated_BecausePasswordIsNull()
        {
            string? password = null;
            TestContext.Out.WriteLine($"[SETUP INFO from Password.TryCreate]: Введен пароль {password ?? "null"}.");
            var passwordResult = Password.TryCreate(password, out Password? passwordVo);

            if (!passwordResult.IsValid)
                for (int i = 0; i < passwordResult.Errors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from Password.TryCreate] - Ошибка:  {passwordResult.Errors[i]}.");

            Assert.That(passwordVo, Is.Null);
        }

        [Test]
        public void Password_TryCreate_WhenPasswordNotCreated_BecausePasswordDoesNotHaveAnUppercaseLetter_AndDoesNotHaveADigit_AndBecausePasswordHasNoCharacters()
        {
            string password = "passwordwithoutupperletter";
            TestContext.Out.WriteLine($"[SETUP INFO from Password.TryCreate]: Введен пароль {password}.");
            var passwordResult = Password.TryCreate(password, out Password? passwordVo);

            if (!passwordResult.IsValid)
                for (int i = 0; i < passwordResult.Errors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from Password.TryCreate] - Ошибка: {passwordResult.Errors[i]}.");

            Assert.That(passwordVo, Is.Null);
        }

        [Test]
        public void Password_TryCreate_WhenPasswordNotCreated_BecausePasswordDoesNotHaveAnLowercaseLetter_AndDoesNotHaveADigit_AndBecausePasswordHasNoCharacters()
        {
            string password = "PASSWORDWITOUTLOWWERLETTER";
            TestContext.Out.WriteLine($"[SETUP INFO from Password.TryCreate]: Введен пароль {password}.");
            var passwordResult = Password.TryCreate(password, out Password? passwordVo);

            if (!passwordResult.IsValid)
                for (int i = 0; i < passwordResult.Errors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from Password.TryCreate] - Ошибка:  {passwordResult.Errors[i]}.");

            Assert.That(passwordVo, Is.Null);
        }

        [Test]
        public void Password_TryCreate_WhenPasswordNotCreated_BecausePasswordDoesNotHaveADigit_AndBecausePasswordHasNoCharacters()
        {
            string password = "PASSWORDwithOUTaDigit";
            TestContext.Out.WriteLine($"[SETUP INFO from Password.TryCreate]: Введен пароль {password}.");
            var passwordResult = Password.TryCreate(password, out Password? passwordVo);

            if (!passwordResult.IsValid)
                for (int i = 0; i < passwordResult.Errors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from Password.TryCreate] - Ошибка:  {passwordResult.Errors[i]}.");

            Assert.That(passwordVo, Is.Null);
        }

        [Test]
        public void Password_TryCreate_WhenPasswordNotCreated_BecausePasswordHasNoCharacters()
        {
            string password = "VeryStrongPasswordWith12345DigitWithOutCharacters"; ;
            TestContext.Out.WriteLine($"[SETUP INFO from Password.TryCreate]: Введен пароль {password}.");
            var passwordResult = Password.TryCreate(password, out Password? passwordVo);

            if (!passwordResult.IsValid)
                for (int i = 0; i < passwordResult.Errors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from Password.TryCreate] - Ошибка:  {passwordResult.Errors[i]}.");

            Assert.That(passwordVo, Is.Null);
        }

        #endregion

        #region PhoneNumber

        [Test]
        public void PhoneNumber_TryCreate_WhenPhoneNumberCreated()
        {
            string phoneNumber = "89004001010";
            //string phoneNumber = "890екркр0400(())))1010";
            var phoneResult = PhoneNumber.TryCreate(phoneNumber, out PhoneNumber? phoneVo);

            Assert.That(phoneVo, Is.Not.Null);
        }

        [Test]
        public void PhoneNumber_TryCreate_WhenPhoneNumberNotCreated_BecausePhoneNumberIsEmpty()
        {
            string phoneNumber = " ";
            TestContext.Out.WriteLine($"[SETUP INFO from PhoneNumber.TryCreate]: Введен номер длинной {phoneNumber.Length} цифр. {phoneNumber}.");
            var phoneResult = PhoneNumber.TryCreate(phoneNumber, out PhoneNumber? phoneVo);

            if (!phoneResult.IsValid)
                for (int i = 0; i < phoneResult.Errors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from PhoneNumber.TryCreate] {phoneResult.Errors[i]}");

            Assert.That(phoneVo, Is.Null);
        }

        [Test]
        public void PhoneNumber_TryCreate_WhenPhoneNumberNotCreated_BecausePhoneNumberIsNull()
        {
            string? phoneNumber = null;
            TestContext.Out.WriteLine($"[SETUP INFO from PhoneNumber.TryCreate]: Введен номер длинной '{phoneNumber ?? "null"}' цифр. {phoneNumber ?? "null"}.");
            var phoneResult = PhoneNumber.TryCreate(phoneNumber, out PhoneNumber? phoneVo);

            if (!phoneResult.IsValid)
                for (int i = 0; i < phoneResult.Errors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from PhoneNumber.TryCreate] {phoneResult.Errors[i]}");

            Assert.That(phoneVo, Is.Null);
        }

        [Test]
        public void PhoneNumber_TryCreate_WhenPhoneNumberNotCreated_BecauseMaximumLengthHasBeenExceeded()
        {
            string phoneNumber = "89004001010467989345668765";
            TestContext.Out.WriteLine($"[SETUP INFO from PhoneNumber.TryCreate]: Введен номер длинной {phoneNumber.Length} цифр. {phoneNumber}.");
            var phoneResult = PhoneNumber.TryCreate(phoneNumber, out PhoneNumber? phoneVo);

            if (!phoneResult.IsValid)
                for (int i = 0; i < phoneResult.Errors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from PhoneNumber.TryCreate] {phoneResult.Errors[i]}");

            Assert.That(phoneVo, Is.Null);
        }

        [Test]
        public void PhoneNumber_TryCreate_WhenPhoneNumberNotCreated_BecauseLengthWasLessThanTheMinimum()
        {
            string phoneNumber = "8900";
            TestContext.Out.WriteLine($"[SETUP INFO from PhoneNumber.TryCreate]: Введен номер длинной {phoneNumber.Length} цифр. {phoneNumber}.");
            var phoneResult = PhoneNumber.TryCreate(phoneNumber, out PhoneNumber? phoneVo);

            if (!phoneResult.IsValid)
                for (int i = 0; i < phoneResult.Errors.Count; i++)
                    TestContext.Out.WriteLine($"[SETUP INFO from PhoneNumber.TryCreate] {phoneResult.Errors[i]}");

            Assert.That(phoneVo, Is.Null);
        }

        #endregion

    }
}
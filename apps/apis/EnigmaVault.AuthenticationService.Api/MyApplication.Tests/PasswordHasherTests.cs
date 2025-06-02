using EnigmaVault.AuthenticationService.Application.Implementations.Hashers;
using Microsoft.Extensions.Logging;
using Moq;

namespace MyApplication.Tests
{
    internal class PasswordHasherTests
    {
        private Argon2PasswordHasher _hasher;
        private Mock<ILogger<Argon2PasswordHasher>> _loggerArgonMock;

        [SetUp]
        public void SetUp()
        {
            _loggerArgonMock = new Mock<ILogger<Argon2PasswordHasher>>();
            _hasher = new Argon2PasswordHasher(_loggerArgonMock.Object);
        }

        private static bool IsValidBase64(string input)
        {
            try
            {
                Convert.FromBase64String(input);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        [Test]
        public void HashPassword_ValidPassword_ReturnsCorrectlyFormattedString()
        {
            //"Argon2id:2:3:65536:someSalt:someHash" - Пример проверяемой строки

            string password = "VeryStrongPassword11!!..";

            var resultHash = _hasher.HashPassword(password);

            string[] parts = resultHash.Split(':');

            Assert.That(parts.Length, Is.EqualTo(6), "Строка должна состоять из 6 частей, разделенных ':'");

            Assert.That(parts[0], Is.EqualTo("Argon2id"), "Первая часть должна быть 'Argon2id'");
            Assert.That(parts[1], Is.EqualTo("2"), "Вторая часть (DegreeOfParallelism) должна быть числом 2");
            Assert.That(parts[2], Is.EqualTo("3"), "Третья часть (Iterations) должна быть числом 3");
            Assert.That(parts[3], Is.EqualTo("65536"), "Четвертая часть (MemorySizeKb) должна быть числом 65536");

            Assert.That(IsValidBase64(parts[4]), Is.True, "Соль не должна быть пустой");
            Assert.That(IsValidBase64(parts[5]), Is.True, "Хеш не должен быть пустым");
        }

        [Test]
        public void HashPassword_ValidPassword_ReturnsCorrectlyFormattedStringWithNumericParts()
        {
            //"Argon2id:2:3:65536:someSalt:someHash" - Пример проверяемой строки

            string password = "VeryStrongPassword11!!..";

            string hashResult = _hasher.HashPassword(password);

            string[] parts = hashResult.Split(':');

            Assert.That(parts.Length, Is.EqualTo(6), "Строка должна состоять из 6 частей, разделенных ':'");

            Assert.That(parts[0], Is.EqualTo("Argon2id"), "Первая часть должна быть 'Argon2id'");

            Assert.That(int.TryParse(parts[1], out _), Is.True, "Вторая часть (DegreeOfParallelism) должна быть валидным int");
            Assert.That(int.TryParse(parts[2], out _), Is.True, "Третья часть (Iterations) должна быть валидным int");
            Assert.That(int.TryParse(parts[3], out _), Is.True, "Четвертая часть (MemorySizeKb) должна быть валидным int");

            Assert.That(IsValidBase64(parts[4]), Is.True, "Соль (первая часть) должна быть валидной Base64 строкой");
            Assert.That(IsValidBase64(parts[5]), Is.True, "Хеш (шестая часть) должен быть валидной Base64 строкой.");

            Assert.That(parts[4], Is.Not.Empty, "Соль не должна быть пустой");
            Assert.That(parts[5], Is.Not.Empty, "Хеш не должен быть пустым");
        }

        [Test]
        public void HashPassword_VerifyPassword_ReturnedTrue()
        {
            string password = "VeryStrongPassword11!!..";
            var hash = _hasher.HashPassword(password);

            var success = _hasher.VerifyPassword(password, hash);

            Assert.That(success, Is.True);
        }

        [Test]
        public void HashPassword_NullPassword_ThrowsArgumentNullException()
        {
            string password = null;

            var ex = Assert.Throws<ArgumentNullException>(() => _hasher.HashPassword(password));
            Assert.That(ex.ParamName, Is.EqualTo("password"));
        }

        [Test]
        public void HashPassword_EmptyPassword_ThrowsArgumentNullException()
        {
            string password = "";

            var ex = Assert.Throws<ArgumentNullException>(() => _hasher.HashPassword(password));
            Assert.That(ex.ParamName, Is.EqualTo("password"));
        }

        [Test]
        public void VerifyPassword_NullPassword_ThrowsArgumentNullException()
        {
            string password = null;
            string storedHashString = "Argon2id:2:3:65536:someSalt:someHash";

            var ex = Assert.Throws<ArgumentNullException>(() => _hasher.VerifyPassword(password, storedHashString));
            Assert.That(ex.ParamName, Is.EqualTo("password"));
        }

        [Test]
        public void VerifyPassword_EmptyPassword_ThrowsArgumentNullException()
        {
            string password = "";
            string storedHashString = "Argon2id:2:3:65536:someSalt:someHash";

            var ex = Assert.Throws<ArgumentNullException>(() => _hasher.VerifyPassword(password, storedHashString));
            Assert.That(ex.ParamName, Is.EqualTo("password"));
        }

        [Test]
        public void VerifyPassword_NullStoredHashString_ThrowsArgumentNullException()
        {
            string password = "ValidPass123";
            string storedHashString = null;

            var ex = Assert.Throws<ArgumentNullException>(() => _hasher.VerifyPassword(password, storedHashString));
            Assert.That(ex.ParamName, Is.EqualTo("storedHashString"));
        }

        [Test]
        public void VerifyPassword_EmptyStoredHashString_ThrowsArgumentNullException()
        {
            string password = "ValidPass123";
            string storedHashString = "";

            var ex = Assert.Throws<ArgumentNullException>(() => _hasher.VerifyPassword(password, storedHashString));
            Assert.That(ex.ParamName, Is.EqualTo("storedHashString"));
        }
    }
}
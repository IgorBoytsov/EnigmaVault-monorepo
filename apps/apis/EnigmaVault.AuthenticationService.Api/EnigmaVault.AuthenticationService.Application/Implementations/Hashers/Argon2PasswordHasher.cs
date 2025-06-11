using EnigmaVault.AuthenticationService.Application.Abstractions.Hashers;
using EnigmaVault.AuthenticationService.Application.DTOs.CryptoParameters;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace EnigmaVault.AuthenticationService.Application.Implementations.Hashers
{
    public class Argon2PasswordHasher(ILogger<Argon2PasswordHasher> logger) : IPasswordHasher
    {
        private readonly ILogger<Argon2PasswordHasher> _logger = logger;

        private const int ArgonDegreeOfParallelism = 2;  // Количество потоков (например, количество ядер CPU)
        private const int ArgonIterations = 3;           // Количество проходов по памяти
        private const int ArgonMemorySizeKb = 65536;     // Объем памяти в КБ (64 МБ). OWASP рекомендует 19MiB (19456 KB) для Argon2id.
                                                         // Увеличивайте по мере возможностей вашего сервера.
        private const int SaltSize = 16;                 // 128 бит
        private const int HashSize = 32;                 // 256 бит

        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                _logger.LogError("Поле password {password} пришло пустое", password);
                throw new ArgumentNullException(nameof(password));
            }
                

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            var argon2 = new Argon2id(passwordBytes)
            {
                Salt = salt,
                DegreeOfParallelism = ArgonDegreeOfParallelism,
                Iterations = ArgonIterations,
                MemorySize = ArgonMemorySizeKb
            };

            byte[] hash = argon2.GetBytes(HashSize);

            string saltBase64 = Convert.ToBase64String(salt);
            string hashBase64 = Convert.ToBase64String(hash);

            _logger.LogDebug("Хеш для пароля был успешно создан.");
            return $"Argon2id:{ArgonDegreeOfParallelism}:{ArgonIterations}:{ArgonMemorySizeKb}:{saltBase64}:{hashBase64}";
        }

        public bool VerifyPassword(string password, string storedHashString)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHashString))
            {
                _logger.LogError("Password или storedHashString пустые.");
                return false;
            }

            var parseResult = ParseHashString(storedHashString);

            if (!parseResult.Success)
            {
                return false; 
            }

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            var argon2 = new Argon2id(passwordBytes)
            {
                Salt = parseResult.Parameters.Salt,
                DegreeOfParallelism = parseResult.Parameters.DegreeOfParallelism,
                Iterations = parseResult.Parameters.Iterations,
                MemorySize = parseResult.Parameters.MemorySizeKb
            };

            byte[] testHash = argon2.GetBytes(parseResult.StoredHash.Length);

            bool verified = CryptographicOperations.FixedTimeEquals(testHash, parseResult.StoredHash);
            if (verified)
            {
                _logger.LogDebug("Верификация хеша была успешно пройдена");
            }

            return verified;
        }

        public CryptoParameter GetParametersFromHash(string storedHashString)
        {
            var parseResult = ParseHashString(storedHashString);
            if (!parseResult.Success)
            {
                return null;
            }

            return parseResult.Parameters;
        }

        private (bool Success, CryptoParameter Parameters, byte[] StoredHash) ParseHashString(string storedHashString)
        {
            if (string.IsNullOrEmpty(storedHashString))
            {
                _logger.LogDebug("storedHashString не может быть пустой.");
                return (false, null, null);
            }

            string[] parts = storedHashString.Split(':');
            if (parts.Length != 6 || parts[0] != "Argon2id")
            {
                _logger.LogDebug("Неверный формат данных для Argon2id: {storedHashString}", storedHashString);
                return (false, null, null);
            }

            try
            {
                var parameters = new CryptoParameter
                {
                    DegreeOfParallelism = int.Parse(parts[1]),
                    Iterations = int.Parse(parts[2]),
                    MemorySizeKb = int.Parse(parts[3]),
                    Salt = Convert.FromBase64String(parts[4])
                };

                byte[] storedHash = Convert.FromBase64String(parts[5]);

                return (true, parameters, storedHash);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Ошибка парсинга компонентов хеша: {storedHashString}", storedHashString);
                return (false, null, null);
            }
        }
    }
}
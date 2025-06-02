using EnigmaVault.AuthenticationService.Application.Abstractions.Hashers;
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
            if (string.IsNullOrEmpty(password))
            {
                _logger.LogError("Поле password {password} пришло пустое", password);
                throw new ArgumentNullException(nameof(password));
            }
                
            if (string.IsNullOrEmpty(storedHashString))
            {
                _logger.LogError("Поле storedHashString {storedHashString} пришло пустое", storedHashString);
                throw new ArgumentNullException(nameof(storedHashString));
            }
               
            string[] parts = storedHashString.Split(':');
            if (parts.Length != 6 || parts[0] != "Argon2id")
            {
                _logger.LogDebug("Не верный формат данных для алгоритма Argon2id. Было получено: {storedHashString}.", storedHashString);
                return false;
            }

            byte[] salt;
            byte[] storedHash;
            int degreeOfParallelism;
            int iterations;
            int memorySizeKb;

            try
            {
                if (!int.TryParse(parts[1], out degreeOfParallelism) ||
                    !int.TryParse(parts[2], out iterations) ||
                    !int.TryParse(parts[3], out memorySizeKb))
                {
                    _logger.LogDebug("Не верные параметры для Argon2. Параметры: {@parts}", parts);
                    return false;
                }

                salt = Convert.FromBase64String(parts[4]);
                storedHash = Convert.FromBase64String(parts[5]);
            }
            catch (FormatException ex)
            {
                _logger.LogDebug(ex, "Ошибка парсинга компонентов хеша");
                return false;
            }

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            var argon2 = new Argon2id(passwordBytes)
            {
                Salt = salt,
                DegreeOfParallelism = degreeOfParallelism,
                Iterations = iterations,
                MemorySize = memorySizeKb
            };

            byte[] testHash = argon2.GetBytes(storedHash.Length);

            _logger.LogDebug("Верификация хеша была успешно пройдена");
            return CryptographicOperations.FixedTimeEquals(testHash, storedHash);
        }
    }
}
using EnigmaVault.AuthenticationService.Application.Abstractions.Hashers;
using Konscious.Security.Cryptography;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace EnigmaVault.AuthenticationService.Application.Implementations.Hashers
{
    public class Argon2PasswordHasher : IPasswordHasher
    {
        private const int ArgonDegreeOfParallelism = 2;  // Количество потоков (например, количество ядер CPU)
        private const int ArgonIterations = 3;           // Количество проходов по памяти
        private const int ArgonMemorySizeKb = 65536;     // Объем памяти в КБ (64 МБ). OWASP рекомендует 19MiB (19456 KB) для Argon2id.
                                                         // Увеличивайте по мере возможностей вашего сервера.
        private const int SaltSize = 16;                 // 128 бит
        private const int HashSize = 32;                 // 256 бит

        public string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));

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

            return $"Argon2id:{ArgonDegreeOfParallelism}:{ArgonIterations}:{ArgonMemorySizeKb}:{saltBase64}:{hashBase64}";
        }

        public bool VerifyPassword(string password, string storedHashString)
        {
            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException(nameof(password));
            if (string.IsNullOrEmpty(storedHashString))
                throw new ArgumentNullException(nameof(storedHashString));

            string[] parts = storedHashString.Split(':');
            if (parts.Length != 6 || parts[0] != "Argon2id")
            {
                Debug.WriteLine("Не верный формат данных для алгоритма Argon2id.");
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
                    Debug.WriteLine("Не верные параметры для Argon2.");
                    return false;
                }

                salt = Convert.FromBase64String(parts[4]);
                storedHash = Convert.FromBase64String(parts[5]);
            }
            catch (FormatException ex)
            {
                Debug.WriteLine($"Ошибка парсинга компонентов хеша: {ex.Message}");
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

            return CryptographicOperations.FixedTimeEquals(testHash, storedHash);
        }
    }
}
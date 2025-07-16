using EnigmaVault.Application.Dtos.Secrets.CryptoService;
using EnigmaVault.Application.Services.Abstractions;
using EnigmaVault.Domain.Constants;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace EnigmaVault.Application.Services.Implementations
{
    public class SecretsCryptoService(ILogger<SecretsCryptoService> logger) : ISecretsCryptoService
    {
        private readonly ILogger<SecretsCryptoService> _logger = logger;

        private byte[]? _encryptionKey;
        private const int AesGcmNonceSize = 12;
        private const int AesGcmTagSize = 16;
        private const int Aes256KeySize = 32;

        public void GenerateEncryptionKey(string masterPassword, CryptoParameters parameters)
        {
            ArgumentNullException.ThrowIfNull(parameters);

            byte[] saltBytes = Convert.FromBase64String(parameters.Salt);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(masterPassword);

            var argon2 = new Argon2id(passwordBytes)
            {
                Salt = saltBytes,
                DegreeOfParallelism = parameters.DegreeOfParallelism,
                Iterations = parameters.Iterations,
                MemorySize = parameters.MemorySizeKb
            };

            _encryptionKey = argon2.GetBytes(Aes256KeySize);
            Array.Clear(passwordBytes, 0, passwordBytes.Length);
        }

        public EncryptedSecret Encrypt(SensitiveSecretData sensitiveData, SecretMetadata metadata)
        {
            ArgumentNullException.ThrowIfNull(metadata);

            var (encryptedData, nonce) = PerformEncryption(sensitiveData);

            return new EncryptedSecret
            {
                ServiceName = metadata.ServiceName,
                Url = metadata.Url!,
                Notes = metadata.Notes!,
                SvgIcon = metadata.SvgIcon,
                IsFavorite = metadata.IsFavorite,

                SchemaVersion = SchemaVersions.CURENT_SCHEMA_VERSION,
                EncryptedData = Convert.ToBase64String(encryptedData),
                Nonce = Convert.ToBase64String(nonce)
            };
        }

        public (string EncryptedData, string Nonce) Encrypt(SensitiveSecretData sensitiveData)
        {
            var (encryptedData, nonce) = PerformEncryption(sensitiveData);
            return (Convert.ToBase64String(encryptedData), Convert.ToBase64String(nonce));
        }

        private (byte[] EncryptedDataWithTag, byte[] Nonce) PerformEncryption(SensitiveSecretData sensitiveData)
        {
            if (_encryptionKey == null)
                throw new InvalidOperationException("Ключ шифрования не установлен.");

            ArgumentNullException.ThrowIfNull(sensitiveData);

            string json = JsonSerializer.Serialize(sensitiveData);
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(json);

            byte[] nonce = RandomNumberGenerator.GetBytes(AesGcmNonceSize);
            byte[] ciphertext = new byte[plaintextBytes.Length];
            byte[] tag = new byte[AesGcmTagSize];

            using (var aesGcm = new AesGcm(_encryptionKey, AesGcmTagSize))
            {
                aesGcm.Encrypt(nonce, plaintextBytes, ciphertext, tag);
            }

            byte[] combinedCiphertextAndTag = new byte[ciphertext.Length + tag.Length];
            Buffer.BlockCopy(ciphertext, 0, combinedCiphertextAndTag, 0, ciphertext.Length);
            Buffer.BlockCopy(tag, 0, combinedCiphertextAndTag, ciphertext.Length, tag.Length);

            return (combinedCiphertextAndTag, nonce);
        }

        public SensitiveSecretData Decrypt(EncryptedSecret? encryptedData)
        {
            if (_encryptionKey == null)
                throw new InvalidOperationException("Ключ шифрования не сгенерирован. Пожалуйста, сначала войдите в систему.");

            if (encryptedData == null)
                throw new InvalidOperationException("Ключ шифрования не сгенерирован. Пожалуйста, сначала войдите в систему.");

            byte[] nonce = Convert.FromBase64String(encryptedData.Nonce);
            byte[] combinedData = Convert.FromBase64String(encryptedData.EncryptedData);

            if (combinedData.Length < AesGcmTagSize)
                throw new CryptographicException("Недопустимые зашифрованные данные: слишком короткие, чтобы содержать тег.");

            byte[] tag = combinedData[^AesGcmTagSize..]; 
            byte[] ciphertext = combinedData[..^AesGcmTagSize];

            byte[] decryptedBytes = new byte[ciphertext.Length];

            try
            {
                using var aesGcm = new AesGcm(_encryptionKey, AesGcmTagSize);
                aesGcm.Decrypt(nonce, ciphertext, tag, decryptedBytes);
            }
            catch (AuthenticationTagMismatchException ex)
            {
                throw new CryptographicException("Не удалось расшифровать данные. Возможно, мастер-пароль введен неверно или данные повреждены.", ex);
            }

            string decryptedJson = Encoding.UTF8.GetString(decryptedBytes);

            _logger.LogInformation("Строка с данными успешно получена.");

            Func<SensitiveSecretData> func = encryptedData.SchemaVersion switch
            {
                1 => () =>
                {
                    var v1Data = JsonSerializer.Deserialize<SensitiveSecretData>(decryptedJson);

                    return v1Data is null
                        ? throw new Exception("Данные не смогли преобразоваться.")
                        : new SensitiveSecretData
                        {
                            Username = v1Data.Username,
                            Password = v1Data.Password,
                            Email = v1Data.Email,
                            RecoveryKeys = v1Data.RecoveryKeys,
                            SecretWord = v1Data.SecretWord,
                        };
                }
                ,
                _ => () => throw new NotSupportedException($"Версия схемы {encryptedData.SchemaVersion} не поддерживается данным приложением. Пожалуйста, обновите до последней версии.")
            };

            return func();
        }

        public void Logout()
        {
            if (_encryptionKey != null)
            {
                Array.Clear(_encryptionKey, 0, _encryptionKey.Length);
                _encryptionKey = null;
            }
        }
    }
}
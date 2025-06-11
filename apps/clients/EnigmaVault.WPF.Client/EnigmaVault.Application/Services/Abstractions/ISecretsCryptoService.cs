using EnigmaVault.Application.Dtos.Secrets.CryptoService;

namespace EnigmaVault.Application.Services.Abstractions
{
    public interface ISecretsCryptoService
    {
        EncryptedSecret Encrypt(SensitiveSecretData sensitiveData, SecretMetadata metadata);
        (string EncryptedData, string Nonce) Encrypt(SensitiveSecretData sensitiveData);
        public SensitiveSecretData Decrypt(EncryptedSecret? payloadFromServer);
        void GenerateEncryptionKey(string masterPassword, CryptoParameters parameters);
        void Logout();
    }
}
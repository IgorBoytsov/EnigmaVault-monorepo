using EnigmaVault.Application.Dtos.Secrets.CryptoService;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.SecretCase
{
    public interface IUpdateEncryptedDataUseCase
    {
        Task<Result<(string EncryptedData, string Nonce, DateTime DateUpdated)>> UpdateEncryptedDataAsync(int idSecret, SensitiveSecretData secretData);
    }
}
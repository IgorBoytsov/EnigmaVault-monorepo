using EnigmaVault.Application.Dtos.Secrets.CryptoService;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.SecretCase
{
    public interface IUpdateSecretUseCase
    {
        Task<Result<(string EncryptedData, string Nonce, DateTime? DateTime)>> UpdateSecretAsync(DecryptedSecret decryptedSecret);
    }
}
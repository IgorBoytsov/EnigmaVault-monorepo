using EnigmaVault.Application.Dtos.Secrets.CryptoService;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.SecretCase
{
    public interface IGetAllSecretsUseCase
    {
        Task<Result<List<EncryptedSecret>?>> GetSecretsAsync(int userId);
    }
}
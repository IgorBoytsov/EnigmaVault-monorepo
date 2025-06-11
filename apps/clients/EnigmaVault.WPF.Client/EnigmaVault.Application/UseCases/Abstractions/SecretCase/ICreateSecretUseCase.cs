using EnigmaVault.Application.Dtos.Secrets.CryptoService;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.SecretCase
{
    public interface ICreateSecretUseCase
    {
        Task<Result<EncryptedSecret>> Create(string serviceName, string username, string password, string email, string secretWord, string? url, string? notes, bool isFavorite);
    }
}
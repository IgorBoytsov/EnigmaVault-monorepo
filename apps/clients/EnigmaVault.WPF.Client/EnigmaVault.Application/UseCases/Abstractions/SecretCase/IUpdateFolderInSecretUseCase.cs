using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.SecretCase
{
    public interface IUpdateFolderInSecretUseCase
    {
        Task<Result> UpdateFolderAsync(int idSecret, int? idFolder);
    }
}
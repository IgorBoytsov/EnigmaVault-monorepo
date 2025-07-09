using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.UseCases.Abstractions.SecretCase;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Implementations.SecretCase
{
    public class UpdateFolderInSecretUseCase(ISecretRepository secretRepository) : IUpdateFolderInSecretUseCase
    {
        private readonly ISecretRepository _secretRepository = secretRepository;

        public async Task<Result> UpdateFolderAsync(int idSecret, int? idFolder) => await _secretRepository.UpdateFolderAsync(idSecret, idFolder);
    }
}
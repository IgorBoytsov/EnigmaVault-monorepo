using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.UseCases.Abstractions.SecretCase;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Implementations.SecretCase
{
    public class UpdateIsArchiveUseCase(ISecretRepository secretRepository) : IUpdateIsArchiveUseCase
    {
        private readonly ISecretRepository _secretRepository = secretRepository;

        public async Task<Result> UpdateArchiveAsync(int idSecret, bool isArchive) => await _secretRepository.UpdateIsArchiveAsync(idSecret, isArchive);
    }
}
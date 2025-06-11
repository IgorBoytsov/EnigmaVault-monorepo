using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.UseCases.Abstractions.SecretCase;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Implementations.SecretCase
{
    public class DeleteSecretUseCase(ISecretRepository secretRepository) : IDeleteSecretUseCase
    {
        private readonly ISecretRepository _secretRepository = secretRepository;

        public async Task<Result<int?>> DeleteAsync(int idSecret) => await _secretRepository.DeleteAsync(idSecret);
    }
}
using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.UseCases.Abstractions.SecretCase;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Implementations.SecretCase
{
    public class UpdateIconInSecretUseCase(ISecretRepository secretRepository) : IUpdateIconInSecretUseCase
    {
        private readonly ISecretRepository _secretRepository = secretRepository;

        public async Task<Result> UpdateAsync(int idSecret, string svgCode) => await _secretRepository.UpdateIconAsync(idSecret, svgCode);
    }
}
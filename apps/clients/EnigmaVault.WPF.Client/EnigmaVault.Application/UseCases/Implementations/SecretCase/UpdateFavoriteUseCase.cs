using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.UseCases.Abstractions.SecretCase;
using EnigmaVault.Domain.Results;
using Microsoft.Extensions.Logging;

namespace EnigmaVault.Application.UseCases.Implementations.SecretCase
{
    public class UpdateFavoriteUseCase(ISecretRepository secretRepository, ILogger<UpdateFavoriteUseCase> logger) : IUpdateFavoriteUseCase
    {
        private readonly ISecretRepository _secretRepository = secretRepository;
        private readonly ILogger<UpdateFavoriteUseCase> _logger = logger;

        public async Task<Result<DateTime>> UpdateFavoriteAsync(int idSecret, bool isFavorite) => await _secretRepository.UpdateFavoriteAsync(idSecret, isFavorite);
    }
}
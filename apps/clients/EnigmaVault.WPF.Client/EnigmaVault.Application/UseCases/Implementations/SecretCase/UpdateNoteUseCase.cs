using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.UseCases.Abstractions.SecretCase;
using EnigmaVault.Domain.Results;
using Microsoft.Extensions.Logging;

namespace EnigmaVault.Application.UseCases.Implementations.SecretCase
{
    public class UpdateNoteUseCase(ISecretRepository secretRepository, ILogger<UpdateNoteUseCase> logger) : IUpdateNoteUseCase
    {
        private readonly ISecretRepository _secretRepository = secretRepository;
        private readonly ILogger<UpdateNoteUseCase> _logger = logger;

        public async Task<Result<DateTime>> UpdateNoteAsync(int idSecret, string? note) => await _secretRepository.UpdateNoteAsync(idSecret, note);
    }
}
using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.UseCases.Abstractions.SecretCase;
using EnigmaVault.Domain.Enums;
using EnigmaVault.Domain.Results;
using Microsoft.Extensions.Logging;

namespace EnigmaVault.Application.UseCases.Implementations.SecretCase
{
    public class UpdateMetaDataUseCase(ISecretRepository secretRepository, ILogger<UpdateMetaDataUseCase> logger) : IUpdateMetaDataUseCase
    {
        private readonly ISecretRepository _secretRepository = secretRepository;
        private readonly ILogger<UpdateMetaDataUseCase> _logger = logger;

        public async Task<Result<DateTime>> UpdateMetaDataAsync(int idSecret, string serviceName, string? url)
        {
            if (idSecret < 0) return Result<DateTime>.Failure(new Error(ErrorCode.EmptyValue, "Вы не передали запись"));
            if (string.IsNullOrWhiteSpace(serviceName)) return Result<DateTime>.Failure(new Error(ErrorCode.EmptyValue, "Вы не заполнили название."));

            return await _secretRepository.UpdateMetaDataAsync(idSecret, serviceName, url);
        }
    }
}
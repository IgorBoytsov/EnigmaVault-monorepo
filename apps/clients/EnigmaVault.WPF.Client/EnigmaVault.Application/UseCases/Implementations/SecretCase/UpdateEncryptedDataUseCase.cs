using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos.Secrets.CryptoService;
using EnigmaVault.Application.Services.Abstractions;
using EnigmaVault.Application.UseCases.Abstractions.SecretCase;
using EnigmaVault.Domain.Results;
using Microsoft.Extensions.Logging;

namespace EnigmaVault.Application.UseCases.Implementations.SecretCase
{
    public class UpdateEncryptedDataUseCase(ISecretRepository secretRepository, ISecretsCryptoService secretsCryptoService, ILogger<UpdateEncryptedDataUseCase> logger) : IUpdateEncryptedDataUseCase
    {
        private readonly ISecretRepository _secretRepository = secretRepository;
        private readonly ISecretsCryptoService _secretsCryptoService = secretsCryptoService;
        private readonly ILogger<UpdateEncryptedDataUseCase> _logger = logger;

        public async Task<Result<(string EncryptedData, string Nonce, DateTime DateUpdated)>> UpdateEncryptedDataAsync(int idSecret, SensitiveSecretData secretData)
        {
            var (EncryptedData, Nonce) = _secretsCryptoService.Encrypt(secretData);
            var resultUpdated = await _secretRepository.UpdateEncryptedDataAsync(idSecret, EncryptedData, Nonce);

            if (!resultUpdated.IsSuccess)
                return Result<ValueTuple<string, string, DateTime>>.Failure(resultUpdated.Errors);

            return Result<ValueTuple<string, string, DateTime>>.Success((EncryptedData, Nonce, resultUpdated.Value));
        }
    }
}
using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos.Secrets.CryptoService;
using EnigmaVault.Application.Services.Abstractions;
using EnigmaVault.Application.UseCases.Abstractions.SecretCase;
using EnigmaVault.Domain.DomainModels;
using EnigmaVault.Domain.Results;
using Microsoft.Extensions.Logging;

namespace EnigmaVault.Application.UseCases.Implementations.SecretCase
{
    public class UpdateSecretUseCase(ISecretRepository secretRepository, ISecretsCryptoService secretsCryptoService, ILogger<UpdateEncryptedDataUseCase> logger) : IUpdateSecretUseCase
    {
        private readonly ISecretRepository _secretRepository = secretRepository;
        private readonly ISecretsCryptoService _secretsCryptoService = secretsCryptoService;
        private readonly ILogger<UpdateEncryptedDataUseCase> _logger = logger;

        public async Task<Result<(string EncryptedData, string Nonce, DateTime? DateTime)>> UpdateSecretAsync(DecryptedSecret decryptedSecret)
        {
            var sensitiveDate = new SensitiveSecretData
            {
                Username = decryptedSecret.Username,
                Password = decryptedSecret.Password,
                Email = decryptedSecret.Email,
                RecoveryKeys = decryptedSecret.RecoveryKeys,
                SecretWord = decryptedSecret.SecretWord,
            };

            var (EncryptedData, Nonce) = _secretsCryptoService.Encrypt(sensitiveDate);

            var secretDomain = SecretDomain.Reconstruct(
                decryptedSecret.IdSecret, decryptedSecret.IdFolder,
                EncryptedData, Nonce,
                decryptedSecret.ServiceName, decryptedSecret.Url, decryptedSecret.Notes, decryptedSecret.SchemaVersion,
                decryptedSecret.DateAdded, decryptedSecret.DateUpdate, decryptedSecret.IsFavorite);

            var resultUpdated = await _secretRepository.UpdateAsync(secretDomain);

            if (!resultUpdated.IsSuccess)
                return Result<ValueTuple<string,string,DateTime?>>.Failure(resultUpdated.Errors);

            return Result<ValueTuple<string, string, DateTime?>>.Success((EncryptedData, Nonce, resultUpdated.Value));
        }
    }
}
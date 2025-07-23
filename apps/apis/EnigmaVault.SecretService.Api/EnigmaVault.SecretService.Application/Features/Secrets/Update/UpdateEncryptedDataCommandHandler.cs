using EnigmaVault.SecretService.Application.Abstractions.Common;
using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Helpers;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed class UpdateEncryptedDataCommandHandler(ISecretRepository secretRepository, IValidationService validator) : IRequestHandler<UpdateEncryptedDataCommand, Result<DateTime>>
    {
        private readonly ISecretRepository _secretRepository = secretRepository;
        private readonly IValidationService _validator = validator;

        public async Task<Result<DateTime>> Handle(UpdateEncryptedDataCommand request, CancellationToken cancellationToken)
        {
            if (RequestGuard.TryGetFailureResult<UpdateEncryptedDataCommand, DateTime>(request, out var nullFailureResult))
                return nullFailureResult;

            var validationResult = await _validator.ValidateAsync(request);

            if (ValidationGuard.TryGetFailureResult<DateTime>(validationResult, out var validationFailureResult))
                return validationFailureResult;

            var storage = await _secretRepository.GetByIdAsync(request.IdSecret, cancellationToken);

            if (storage is null)
                return Result<DateTime>.Failure(new Error(ErrorCode.NotFound, "Данной записи не существует."));

            try
            {
                storage.UpdateEncryptedPayload(request.EncryptedData!, request.Nonce!, request.SchemaVersion);

                var result = await _secretRepository.UpdateAsync(storage);

                return result;
            }
            catch (ArgumentNullException ex)
            {
                return Result<DateTime>.Failure(new Error(ErrorCode.DomainError, $"Ошибка обновление зашифрованных данных в домене. Вероятно они были равны null. Исключение: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return Result<DateTime>.Failure(new Error(ErrorCode.UnknownError, $"Неизвестная ошибка. Исключение: {ex.Message}"));
            }
        }
    }
}
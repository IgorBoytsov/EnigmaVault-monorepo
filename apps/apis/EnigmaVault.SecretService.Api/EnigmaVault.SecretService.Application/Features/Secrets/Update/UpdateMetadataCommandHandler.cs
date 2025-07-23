using EnigmaVault.SecretService.Application.Abstractions.Common;
using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Helpers;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed class UpdateMetadataCommandHandler(ISecretRepository secretRepository, IValidationService validator) : IRequestHandler<UpdateMetadataCommand, Result<DateTime>>
    {
        private readonly ISecretRepository _secretRepository = secretRepository;
        private readonly IValidationService _validator = validator;

        public async Task<Result<DateTime>> Handle(UpdateMetadataCommand request, CancellationToken cancellationToken)
        {
            if (RequestGuard.TryGetFailureResult<UpdateMetadataCommand, DateTime>(request, out var nullFailureResult))
                return nullFailureResult;

            var validationResult = await _validator.ValidateAsync(request);

            if (ValidationGuard.TryGetFailureResult<DateTime>(validationResult, out var validationFailureResult))
                return validationFailureResult;

            var storage = await _secretRepository.GetByIdAsync(request.IdSecret, cancellationToken);

            if (storage is null)
                return Result<DateTime>.Failure(new Error(ErrorCode.NotFound, "Данной записи не существует."));

            try
            {
                storage.UpdateMetadata(request.ServiceName, request.Url);

                var result = await _secretRepository.UpdateAsync(storage);

                return result;
            }
            catch(ArgumentNullException ex) 
            {
                return Result<DateTime>.Failure(new Error(ErrorCode.DomainError, $"Ошибка обновление мета данных в домене. Вероятно они были равны null. Исключение: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return Result<DateTime>.Failure(new Error(ErrorCode.UnknownError, $"Неизвестная ошибка. Исключение: {ex.Message}"));
            }
        }
    }
}
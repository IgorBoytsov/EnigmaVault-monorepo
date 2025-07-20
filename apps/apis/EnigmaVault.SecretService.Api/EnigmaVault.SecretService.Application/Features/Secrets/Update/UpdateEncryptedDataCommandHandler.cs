using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public class UpdateEncryptedDataCommandHandler(ISecretRepository secretRepository) : IRequestHandler<UpdateEncryptedDataCommand, Result<DateTime>>
    {
        private readonly ISecretRepository _secretRepository = secretRepository;

        public async Task<Result<DateTime>> Handle(UpdateEncryptedDataCommand request, CancellationToken cancellationToken)
        {
            var errors = new List<Error>();

            if (request is null)
                return Result<DateTime>.Failure(new Error(ErrorCode.NullValue, $"Команда была пустая"));

            if (request.EncryptedData is null)
                errors.Add(new Error(ErrorCode.Empty, $"Не были указаны зашифрованные данные."));

            if (request.Nonce is null)
                errors.Add(new Error(ErrorCode.Empty, $"Не были указан Nonce."));

            if (request.SchemaVersion <= 0)
                errors.Add(new Error(ErrorCode.Empty, $"Версия схемы указа не корректно. Самая минимальная версия (1)."));

            if (errors.Any())
                return Result<DateTime>.Failure(errors);

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
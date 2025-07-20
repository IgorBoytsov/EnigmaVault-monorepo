using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public class UpdateMetadataCommandHandler(ISecretRepository secretRepository) : IRequestHandler<UpdateMetadataCommand, Result<DateTime>>
    {
        private readonly ISecretRepository _secretRepository = secretRepository;

        public async Task<Result<DateTime>> Handle(UpdateMetadataCommand request, CancellationToken cancellationToken)
        {
            var errors = new List<Error>();

            if (request is null)
                return Result<DateTime>.Failure(new Error(ErrorCode.NullValue, $"Значение {nameof(request)} было пустым"));

            if (string.IsNullOrWhiteSpace(request.ServiceName))
                errors.Add(new Error(ErrorCode.Empty, $"Не было указано название записи."));

            if (errors.Any())
                return Result<DateTime>.Failure(errors);

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
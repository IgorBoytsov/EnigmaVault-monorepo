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

            if (!await _secretRepository.ExistSecret(request.IdSecret))
                errors.Add(new Error(ErrorCode.NotFound, "Данной записи не существует."));

            if (string.IsNullOrWhiteSpace(request.ServiceName))
                errors.Add(new Error(ErrorCode.Empty, $"Не было указано название записи."));

            if (errors.Any())
                return Result<DateTime>.Failure(errors);

            var result = await _secretRepository.UpdateMetadataAsync(request);

            return result;
        }
    }
}
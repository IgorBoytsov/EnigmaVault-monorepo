using EnigmaVault.SecretService.Application.Abstractions.Common;
using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Helpers;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed class UpdateSecretFolderCommandHandler(ISecretRepository secretRepository, IValidationService validator) : IRequestHandler<UpdateSecretFolderCommand, Result>
    {
        public readonly ISecretRepository _secretRepository = secretRepository;
        public readonly IValidationService _validator = validator;

        public async Task<Result> Handle(UpdateSecretFolderCommand request, CancellationToken cancellationToken)
        {
            if (RequestGuard.TryGetFailureResult<UpdateSecretFolderCommand>(request, out var nullFailureResult))
                return nullFailureResult;

            var validationResult = await _validator.ValidateAsync(request);

            if (ValidationGuard.TryGetFailureResult(validationResult, out var validationFailureResult))
                return validationFailureResult;

            var storage = await _secretRepository.GetByIdAsync(request.IdSecret, cancellationToken);

            if (storage is null)
                return Result<DateTime>.Failure(new Error(ErrorCode.NotFound, "Данной записи не существует."));

            storage.UpdateFolder(request.IdFolder);

            var result = await _secretRepository.UpdateAsync(storage);

            return result;
        }
    }
}
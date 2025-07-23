using EnigmaVault.SecretService.Application.Abstractions.Common;
using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Helpers;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed class UpdateSecretCommandHandler(ISecretRepository secretRepository, IValidationService validator) : IRequestHandler<UpdateSecretCommand, Result<DateTime>>
    {
        private readonly ISecretRepository _secretRepository = secretRepository;
        private readonly IValidationService _validator = validator;

        public async Task<Result<DateTime>> Handle(UpdateSecretCommand request, CancellationToken cancellationToken)
        {
            if (RequestGuard.TryGetFailureResult<UpdateSecretCommand, DateTime>(request, out var nullFailureResult))
                return nullFailureResult;

            var validationResult = await _validator.ValidateAsync(request);

            if (ValidationGuard.TryGetFailureResult<DateTime>(validationResult, out var validationFailureResult))
                return validationFailureResult;

            var secret = await _secretRepository.GetByIdAsync(request.IdSecret, cancellationToken);

            if (secret is null)
                return Result<DateTime>.Failure(new Error(ErrorCode.NullValue, "Значение запроса было пустым."));

            secret.UpdateNote(request.Note);
            secret.UpdateMetadata(request.ServiceName ?? secret.ServiceName, request.Url);
            secret.UpdateFavoriteStatus(request.IsFavorite!.Value);
            secret.UpdateEncryptedPayload(request.EncryptedData!, request.Nonce!, request.SchemaVersion);

            var result = await _secretRepository.UpdateAsync(secret);

            return result;
        }
    }
}
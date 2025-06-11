using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public class UpdateSecretCommandHandler(ISecretRepository secretRepository) : IRequestHandler<UpdateSecretCommand, Result<DateTime>>
    {
        private readonly ISecretRepository _secretRepository = secretRepository;

        public async Task<Result<DateTime>> Handle(UpdateSecretCommand request, CancellationToken cancellationToken)
        {
            var secret = await _secretRepository.GetByIdAsync(request.IdSecret, cancellationToken);

            if (secret is null)
                return Result<DateTime>.Failure(new Error(ErrorCode.NullValue, "Значение запроса было пустым."));

            secret.UpdateNote(request.Note);
            secret.UpdateMetadata(request.ServiceName ?? secret.ServiceName, request.Url);
            secret.UpdateFavoriteStatus(request.IsFavorite!.Value);
            secret.UpdateEncryptedPayload(Convert.FromBase64String(request.EncryptedData!), Convert.FromBase64String(request.Nonce!), request.SchemaVersion!.Value);

            var result = await _secretRepository.UpdateAsync(secret);

            return result;
        }
    }
}
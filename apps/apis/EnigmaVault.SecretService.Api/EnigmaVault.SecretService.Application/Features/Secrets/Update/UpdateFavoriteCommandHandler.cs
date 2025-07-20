using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed class UpdateFavoriteCommandHandler(ISecretRepository secretRepository) : IRequestHandler<UpdateFavoriteCommand, Result<DateTime>>
    {
        private readonly ISecretRepository _secretRepository = secretRepository;

        public async Task<Result<DateTime>> Handle(UpdateFavoriteCommand request, CancellationToken cancellationToken)
        {
            var storage = await _secretRepository.GetByIdAsync(request.IdSecret, cancellationToken);

            if (storage is null)
                return Result<DateTime>.Failure(new Error(ErrorCode.NotFound, $"Данной записи не существует."));

            storage.ToggleFavorite(request.IsFavorite);

            var result = await _secretRepository.UpdateAsync(storage);

            return result;
        }
    }
}
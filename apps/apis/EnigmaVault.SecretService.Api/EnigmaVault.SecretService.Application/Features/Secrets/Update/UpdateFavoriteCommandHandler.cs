using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public class UpdateFavoriteCommandHandler(ISecretRepository secretRepository) : IRequestHandler<UpdateFavoriteCommand, Result<DateTime>>
    {
        private readonly ISecretRepository _secretRepository = secretRepository;

        public async Task<Result<DateTime>> Handle(UpdateFavoriteCommand request, CancellationToken cancellationToken)
        {
            if (!await _secretRepository.ExistSecret(request.IdSecret))
                return Result<DateTime>.Failure(new Error(ErrorCode.NotFound, $"Данной записи не существует."));

            var result = await _secretRepository.UpdateFavoriteAsync(request);

            return result;
        }
    }
}
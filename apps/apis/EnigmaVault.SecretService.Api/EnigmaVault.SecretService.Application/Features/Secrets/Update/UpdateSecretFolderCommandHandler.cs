using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public class UpdateSecretFolderCommandHandler(ISecretRepository secretRepository) : IRequestHandler<UpdateSecretFolderCommand, Result>
    {
        public readonly ISecretRepository _secretRepository = secretRepository;

        public async Task<Result> Handle(UpdateSecretFolderCommand request, CancellationToken cancellationToken)
        {
            var result = await _secretRepository.UpdateFolderAsync(request);

            return result;
        }
    }
}
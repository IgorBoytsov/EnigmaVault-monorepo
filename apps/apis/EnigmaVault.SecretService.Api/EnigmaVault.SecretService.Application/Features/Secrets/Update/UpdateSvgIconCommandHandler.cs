using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public class UpdateSvgIconCommandHandler(ISecretRepository secretRepository) : IRequestHandler<UpdateSvgIconCommand, Result<DateTime>>
    {
        public readonly ISecretRepository _secretRepository = secretRepository;

        public async Task<Result<DateTime>> Handle(UpdateSvgIconCommand request, CancellationToken cancellationToken) => await _secretRepository.UpdateSvgIconAsync(request);
    }
}
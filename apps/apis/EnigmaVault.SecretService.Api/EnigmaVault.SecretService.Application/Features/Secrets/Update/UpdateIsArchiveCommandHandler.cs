using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed class UpdateIsArchiveCommandHandler(ISecretRepository secretRepository) : IRequestHandler<UpdateIsArchiveCommand, Result>
    {
        private readonly ISecretRepository _secretRepository = secretRepository;

        public async Task<Result> Handle(UpdateIsArchiveCommand request, CancellationToken cancellationToken) => await _secretRepository.UpdateIsArchiveAsync(request);
    }
}
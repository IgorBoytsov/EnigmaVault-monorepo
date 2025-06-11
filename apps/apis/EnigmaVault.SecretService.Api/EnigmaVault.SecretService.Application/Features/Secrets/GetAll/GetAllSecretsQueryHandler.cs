using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.GetAll
{
    public class GetAllSecretsQueryHandler(ISecretRepository secretRepository) : IRequestHandler<GetAllSecretsQuery, IAsyncEnumerable<SecretDto>>
    {
        private readonly ISecretRepository _secretRepository = secretRepository;

        public Task<IAsyncEnumerable<SecretDto>> Handle(GetAllSecretsQuery request, CancellationToken cancellationToken)
            => Task.FromResult(_secretRepository.GetAllStreamingAsync(request.UserId, cancellationToken));
    }
}
using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using MediatR;
using System.Runtime.CompilerServices;

namespace EnigmaVault.SecretService.Application.Features.Secrets.GetAll
{
    public class GetAllSecretsQueryHandler(ISecretRepository secretRepository) : IStreamRequestHandler<GetAllSecretsQuery, SecretDto>
    {
        private readonly ISecretRepository _secretRepository = secretRepository;

        public async IAsyncEnumerable<SecretDto> Handle(GetAllSecretsQuery request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var secretsStream = _secretRepository.GetAllStreamingAsync(request.UserId, cancellationToken);

            await foreach (var secret in secretsStream.WithCancellation(cancellationToken))
                yield return secret;
        }
    }
}
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.GetAll
{
    public sealed record GetAllSecretsQuery(int UserId) : IRequest<IAsyncEnumerable<SecretDto>>;
}
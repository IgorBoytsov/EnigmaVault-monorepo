using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.GetAll
{
    public class GetAllSecretsQuery : IRequest<IAsyncEnumerable<SecretDto>>
    {
        public int UserId { get; set; }
    }
}
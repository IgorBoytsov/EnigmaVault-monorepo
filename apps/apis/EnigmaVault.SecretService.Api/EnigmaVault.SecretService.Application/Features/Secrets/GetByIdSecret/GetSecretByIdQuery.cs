using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.GetByIdSecret
{
    public class GetSecretByIdQuery : IRequest<Result<SecretDto>>
    {
        public int IdSecret { get; set; }
    }
}
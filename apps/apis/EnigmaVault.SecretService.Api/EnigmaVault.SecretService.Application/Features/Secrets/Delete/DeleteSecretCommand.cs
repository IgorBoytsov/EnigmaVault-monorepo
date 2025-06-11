using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Delete
{
    public class DeleteSecretCommand : IRequest<Result>
    {
        public int IdSecret { get; set; }
    }
}
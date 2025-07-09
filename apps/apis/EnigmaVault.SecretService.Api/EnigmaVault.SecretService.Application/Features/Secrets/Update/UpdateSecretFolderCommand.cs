using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public class UpdateSecretFolderCommand : IRequest<Result>
    {
        public int IdSecret { get; set; }
        public int? IdFolder { get; set; }
    }
}
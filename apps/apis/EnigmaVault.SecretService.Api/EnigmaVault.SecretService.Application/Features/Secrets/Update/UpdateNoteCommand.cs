using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public class UpdateNoteCommand : IRequest<Result<DateTime>>
    {
        public int IdSecret { get; set; }

        public string? Note { get; set; }
    }
}
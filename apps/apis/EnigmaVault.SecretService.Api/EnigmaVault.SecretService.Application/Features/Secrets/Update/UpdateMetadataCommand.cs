using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public class UpdateMetadataCommand : IRequest<Result<DateTime>>
    {
        public int IdSecret { get; set; }

        public string ServiceName { get; set; } = null!;

        public string? Url { get; set; }
    }
}
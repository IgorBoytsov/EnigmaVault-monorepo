using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Create
{
    public class CreateSecretCommand : IRequest<Result<SecretDto>>
    {
        public int IdUser { get; set; }

        public byte[] EncryptedData { get; set; } = null!;

        public byte[] Nonce { get; set; } = null!;

        public string ServiceName { get; set; } = null!;

        public string? Url { get; set; }

        public string? Notes { get; set; }

        public string? SvgIcon { get;  set; }

        public int SchemaVersion { get; set; }

        public bool IsFavorite { get; set; }
    }
}
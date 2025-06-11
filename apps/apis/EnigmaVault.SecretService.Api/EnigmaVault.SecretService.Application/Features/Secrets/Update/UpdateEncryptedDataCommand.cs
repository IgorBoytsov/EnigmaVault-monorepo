using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public class UpdateEncryptedDataCommand : IRequest<Result<DateTime>>
    {
        public int IdSecret { get; set; }

        public byte[] EncryptedData { get; set; } = null!;

        public byte[] Nonce { get; set; } = null!;

        public int SchemaVersion { get; set; }
    }
}

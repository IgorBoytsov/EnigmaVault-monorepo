using EnigmaVault.SecretService.Domain.Results;
using MediatR;
using System.Text.Json.Serialization;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public class UpdateSecretCommand : IRequest<Result<DateTime>>
    {
        [JsonIgnore]
        public int IdSecret { get; set; }

        public string? ServiceName { get; set; }

        public string? Url { get; set; }

        public bool? IsFavorite { get; set; }

        public string? Note { get; set; }

        public string? EncryptedData { get; set; }

        public string? Nonce { get; set; }

        public int? SchemaVersion { get; set; }
    }
}
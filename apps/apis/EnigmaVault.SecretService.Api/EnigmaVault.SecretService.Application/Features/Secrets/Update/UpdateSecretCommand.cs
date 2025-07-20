using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed record UpdateSecretCommand(int IdSecret, string? ServiceName, string? Url, bool? IsFavorite, string? Note, string? EncryptedData, string? Nonce, int? SchemaVersion) : IRequest<Result<DateTime>>;
}
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Create
{
    public sealed record CreateSecretCommand(int IdUser, byte[] EncryptedData, byte[] Nonce, string ServiceName, string? Url, string? Notes, string? SvgIcon, int SchemaVersion, bool IsFavorite) : IRequest<Result<SecretDto>>;
}
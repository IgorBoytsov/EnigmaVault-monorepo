using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed record UpdateEncryptedDataCommand(int IdSecret, byte[] EncryptedData, byte[] Nonce, int SchemaVersion) : IRequest<Result<DateTime>>;
}
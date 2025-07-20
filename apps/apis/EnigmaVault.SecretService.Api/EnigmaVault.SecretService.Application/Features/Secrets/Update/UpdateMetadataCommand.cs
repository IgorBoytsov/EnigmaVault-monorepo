using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed record UpdateMetadataCommand(int IdSecret, string ServiceName, string? Url) : IRequest<Result<DateTime>>;
}
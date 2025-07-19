using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed record UpdateIsArchiveCommand(int IdSecret, bool IsArchive) : IRequest<Result>;
}
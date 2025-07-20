using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed record UpdateNoteCommand(int IdSecret, string? Note) : IRequest<Result<DateTime>>;
}
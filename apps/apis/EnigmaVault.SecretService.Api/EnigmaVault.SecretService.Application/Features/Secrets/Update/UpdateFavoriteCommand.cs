using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed record UpdateFavoriteCommand(int IdSecret, bool IsFavorite) : IRequest<Result<DateTime>>;
}
using EnigmaVault.SecretService.Application.Features.Secrets.Validators.Abstractions;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed record UpdateFavoriteCommand(int IdSecret, bool IsFavorite) : IRequest<Result<DateTime>>, IIdSecretDataHolder;
}
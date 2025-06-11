using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public class UpdateFavoriteCommand : IRequest<Result<DateTime>>
    {
        public int IdSecret { get; set; }

        public bool IsFavorite { get; set; }
    }
}
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.SecretCase
{
    public interface IUpdateFavoriteUseCase
    {
        Task<Result<DateTime>> UpdateFavoriteAsync(int idSecret, bool isFavorite);
    }
}
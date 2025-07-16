using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.IconCase
{
    public interface IUpdateIconUseCase
    {
        Task<Result> UpdateNameAsync(int idUser, int idIcon, string name, string? svgCode);
    }
}
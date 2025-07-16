using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.SecretCase
{
    public interface IUpdateIconInSecretUseCase
    {
        Task<Result> UpdateAsync(int idSecret, string svgCode);
    }
}
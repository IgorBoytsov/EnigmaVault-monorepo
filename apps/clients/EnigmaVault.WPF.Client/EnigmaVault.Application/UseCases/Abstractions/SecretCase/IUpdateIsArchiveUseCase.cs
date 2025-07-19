using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.SecretCase
{
    public interface IUpdateIsArchiveUseCase
    {
        Task<Result> UpdateArchiveAsync(int idSecret, bool isArchive);
    }
}
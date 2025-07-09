using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.FolderCase
{
    public interface IUpdateFolderNameUseCase
    {
        Task<Result> UpdateNameAsync(int idFolder, string newName);
    }
}
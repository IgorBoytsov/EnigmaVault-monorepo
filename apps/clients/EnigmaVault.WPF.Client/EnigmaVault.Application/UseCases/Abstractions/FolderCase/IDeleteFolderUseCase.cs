using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.FolderCase
{
    public interface IDeleteFolderUseCase
    {
        Task<Result> DeleteAsync(int idFolder);
    }
}
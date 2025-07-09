using EnigmaVault.Application.Dtos.Secrets.Folders;
using EnigmaVault.Domain.DomainModels;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.Abstractions.Repositories
{
    public interface IFolderRepository
    {
        Task<Result<FolderDomain>> CreateAsync(FolderDomain folder);
        Task<Result<List<FolderDto>>> GetAllAsync(int userId);
        Task<Result> UpdateFolderNameAsync(int idFolder, string newName);
        Task<Result> DeleteAsync(int idFolder);
    }
}
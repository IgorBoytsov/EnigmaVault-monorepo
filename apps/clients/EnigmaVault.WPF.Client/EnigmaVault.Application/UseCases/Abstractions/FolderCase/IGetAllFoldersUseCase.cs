using EnigmaVault.Application.Dtos.Secrets.Folders;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.FolderCase
{
    public interface IGetAllFoldersUseCase
    {
        Task<Result<List<FolderDto>>> GetAllAsync(int userId);
    }
}
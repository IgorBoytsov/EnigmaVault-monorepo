using EnigmaVault.Application.Dtos.Secrets.Folders;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.FolderCase
{
    public interface ICreateFolderUseCase
    {
        Task<Result<FolderDto>> CreateAsync(int idUser, string name);
    }
}
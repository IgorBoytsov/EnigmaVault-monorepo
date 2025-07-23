using EnigmaVault.SecretService.Application.Features.Folders;
using EnigmaVault.SecretService.Application.Features.Folders.Delete;
using EnigmaVault.SecretService.Application.Features.Folders.Update;
using EnigmaVault.SecretService.Domain.DomainModels;
using EnigmaVault.SecretService.Domain.Results;
using System.Runtime.CompilerServices;

namespace EnigmaVault.SecretService.Application.Abstractions.Repositories
{
    public interface IFolderRepository
    {
        Task<Result<FolderDomain>> Create(FolderDomain folder);
        IAsyncEnumerable<FolderDto> GetAllStreamingAsync(int idUser, [EnumeratorCancellation] CancellationToken cancellationToken = default);
        Task<Result> UpdateName(UpdateFolderCommand command);
        Task<Result> Delete(DeleteFolderCommand command);
        Task<bool> ExistSecretsInFolder(int folderId);
        Task<bool> ExistAsync(int folderId);
    }
}
using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.UseCases.Abstractions.FolderCase;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Implementations.FolderCase
{
    public class DeleteFolderUseCase(IFolderRepository folderRepository) : IDeleteFolderUseCase
    {
        private readonly IFolderRepository _folderRepository = folderRepository;

        public async Task<Result> DeleteAsync(int idFolder) => await _folderRepository.DeleteAsync(idFolder);
    }
}
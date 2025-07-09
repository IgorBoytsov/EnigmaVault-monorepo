using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos.Secrets.Folders;
using EnigmaVault.Application.UseCases.Abstractions.FolderCase;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Implementations.FolderCase
{
    public class GetAllFoldersUseCase(IFolderRepository folderRepository) : IGetAllFoldersUseCase
    {
        private readonly IFolderRepository _folderRepository = folderRepository;

        public async Task<Result<List<FolderDto>>> GetAllAsync(int userId) => await _folderRepository.GetAllAsync(userId);
    }
}
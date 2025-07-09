using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.UseCases.Abstractions.FolderCase;
using EnigmaVault.Domain.Enums;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Implementations.FolderCase
{
    public class UpdateFolderNameUseCase(IFolderRepository folderRepository) : IUpdateFolderNameUseCase
    {
        private readonly IFolderRepository _folderRepository = folderRepository;

        public async Task<Result> UpdateNameAsync(int idFolder, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                return Result.Failure(new Error(ErrorCode.EmptyValue, "Название папки не может быть пустым"));

            return await _folderRepository.UpdateFolderNameAsync(idFolder, newName);
        }
    }
}
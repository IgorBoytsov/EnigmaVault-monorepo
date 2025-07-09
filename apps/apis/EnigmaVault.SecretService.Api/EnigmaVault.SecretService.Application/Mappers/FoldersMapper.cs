using EnigmaVault.SecretService.Application.Features.Folders;
using EnigmaVault.SecretService.Domain.DomainModels;

namespace EnigmaVault.SecretService.Application.Mappers
{
    public static class FoldersMapper
    {
        public static FolderDto ToDto(this FolderDomain domain)
        {
            return new FolderDto
            {
                IdFolder = domain.IdFolder,
                IdUser = domain.IdUser,
                FolderName = domain.FolderName,
            };
        }
    }
}
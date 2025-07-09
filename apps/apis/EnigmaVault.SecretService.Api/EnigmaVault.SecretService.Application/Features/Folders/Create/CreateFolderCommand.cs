using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Folders.Create
{
    public class CreateFolderCommand : IRequest<Result<FolderDto>>
    {
        public int IdUser { get; set; }

        public string FolderName { get; set; } = null!;
    }
}
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Folders.Delete
{
    public class DeleteFolderCommand : IRequest<Result>
    {
        public int FolderId { get; set; }
    }
}
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Folders.GetAll
{
    public class GetAllFoldersQuery : IRequest<IAsyncEnumerable<FolderDto>>
    {
        public int UserId { get; set; }
    }
}
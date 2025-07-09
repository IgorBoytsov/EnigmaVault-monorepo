using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Folders.GetAll
{
    public class GetAllFoldersQueryHandler(IFolderRepository folderRepository) : IRequestHandler<GetAllFoldersQuery, IAsyncEnumerable<FolderDto>>
    {
        private readonly IFolderRepository _folderRepository = folderRepository;

        public Task<IAsyncEnumerable<FolderDto>> Handle(GetAllFoldersQuery request, CancellationToken cancellationToken)
            => Task.FromResult(_folderRepository.GetAllStreamingAsync(request.UserId, cancellationToken));
    }
}
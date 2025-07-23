using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using MediatR;
using System.Runtime.CompilerServices;

namespace EnigmaVault.SecretService.Application.Features.Folders.GetAll
{
    public sealed class GetAllFoldersQueryHandler(IFolderRepository folderRepository) : IStreamRequestHandler<GetAllFoldersQuery, FolderDto>
    {
        private readonly IFolderRepository _folderRepository = folderRepository;

        //TODO: Реализовать валидацию с исключениями. В API перехватывать их через Middleware
        public async IAsyncEnumerable<FolderDto> Handle(GetAllFoldersQuery request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var iconsStream = _folderRepository.GetAllStreamingAsync(request.UserId, cancellationToken);

            await foreach (var icon in iconsStream.WithCancellation(cancellationToken))
                yield return icon;
        }
    }
}
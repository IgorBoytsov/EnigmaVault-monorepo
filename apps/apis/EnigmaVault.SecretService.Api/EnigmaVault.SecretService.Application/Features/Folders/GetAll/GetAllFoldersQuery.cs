using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Folders.GetAll
{
    public sealed record GetAllFoldersQuery(int UserId) : IRequest<IAsyncEnumerable<FolderDto>>;
}
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Folders.Delete
{
    public sealed record DeleteFolderCommand(int FolderId) : IRequest<Result>;
}
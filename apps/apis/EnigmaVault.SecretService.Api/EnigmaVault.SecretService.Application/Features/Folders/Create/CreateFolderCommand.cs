using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Folders.Create
{
    public sealed record CreateFolderCommand(int IdUser, string FolderName) : IRequest<Result<FolderDto>>;
}
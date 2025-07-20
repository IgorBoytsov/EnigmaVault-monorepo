using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Folders.Update
{
    public sealed record UpdateFolderCommand(int IdFolder, string Name) : IRequest<Result>;
}
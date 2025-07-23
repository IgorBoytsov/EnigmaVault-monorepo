using EnigmaVault.SecretService.Application.Features.Folders.Validators.Abstractions;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Folders.Delete
{
    public sealed record DeleteFolderCommand(int IdFolder) : IRequest<Result>, IIdFolderValidator;
}
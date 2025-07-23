using EnigmaVault.SecretService.Application.Features.Folders.Validators.Abstractions;
using EnigmaVault.SecretService.Application.Features.SharedValidator.Abstractions;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Folders.Create
{
    public sealed record CreateFolderCommand(int UserId, string FolderName) : IRequest<Result<FolderDto>>, IFolderNameValidator, IMustHaveUser;
}
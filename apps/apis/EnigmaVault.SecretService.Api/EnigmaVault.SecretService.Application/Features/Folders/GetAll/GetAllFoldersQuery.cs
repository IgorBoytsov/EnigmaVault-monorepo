using EnigmaVault.SecretService.Application.Features.SharedValidator.Abstractions;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Folders.GetAll
{
    public sealed record GetAllFoldersQuery(int UserId) : IStreamRequest<FolderDto>, IMustHaveUser;
}
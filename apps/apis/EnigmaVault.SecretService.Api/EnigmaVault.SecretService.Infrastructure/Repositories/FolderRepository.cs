using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Features.Folders;
using EnigmaVault.SecretService.Application.Features.Folders.Delete;
using EnigmaVault.SecretService.Application.Features.Folders.Update;
using EnigmaVault.SecretService.Domain.DomainModels;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using EnigmaVault.SecretService.Infrastructure.Data;
using EnigmaVault.SecretService.Infrastructure.Data.Entities;
using EnigmaVault.SecretService.Infrastructure.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace EnigmaVault.SecretService.Infrastructure.Repositories
{
    internal class FolderRepository(SecretDBContext context, IEntityUpdater entityUpdater) : IFolderRepository
    {
        private readonly SecretDBContext _context = context;
        private readonly IEntityUpdater _entityUpdater = entityUpdater;

        /*--Create----------------------------------------------------------------------------------------*/

        public async Task<Result<FolderDomain>> Create(FolderDomain folder)
        {
            try
            {
                var entity = new Folder
                {
                    IdUser = folder.IdUser,
                    FolderName = folder.FolderName,
                };

                await _context.Folders.AddAsync(entity);
                await _context.SaveChangesAsync();

                var domain = FolderDomain.Reconstruct(entity.IdFolder, entity.IdUser, entity.FolderName);

                return Result<FolderDomain>.Success(domain);
            }
            catch (Exception)
            {
                return Result<FolderDomain>.Failure(new Error(ErrorCode.SaveError, "Не удалось создать папку."));
            }
        }

        /*--Get-------------------------------------------------------------------------------------------*/

        public async IAsyncEnumerable<FolderDto> GetAllStreamingAsync(int idUser, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var dtosStream = _context.Folders
                .AsNoTracking()
                .Where(s => s.IdUser == idUser)
                .Select(s => new FolderDto
                {
                    IdUser = s.IdUser,
                    IdFolder = s.IdFolder,
                    FolderName = s.FolderName,
                })
                .AsAsyncEnumerable()
                .WithCancellation(cancellationToken);

            await foreach (var dto in dtosStream)
                yield return dto;
        }

        /*--Update----------------------------------------------------------------------------------------*/

        public async Task<Result> UpdateName(UpdateFolderCommand command) 
            => await _entityUpdater.UpdatePropertyAsync<Folder>(
                predicate: folder => folder.IdFolder == command.IdFolder,
                setPropertyCalls: f => f
                    .SetProperty(p => p.FolderName, command.Name));

        /*--Delete----------------------------------------------------------------------------------------*/

        public async Task<Result> Delete(DeleteFolderCommand command)
        {
            if (await ExistSecretsInFolder(command.FolderId))
            {
                await _entityUpdater.UpdatePropertyAsync<Secret>(
                predicate: secret => secret.IdFolder == command.FolderId,
                setPropertyCalls: s => s
                    .SetProperty(p => p.IdFolder, (int?)null));
            }

            var deleteResult = await _context.Folders.Where(f => f.IdFolder == command.FolderId).ExecuteDeleteAsync();

            if (deleteResult > 0)
                return Result.Success();

            return Result.Failure(new Error(ErrorCode.DeleteError, "Не удалось удалить папку. Возможно ее уже не существует."));
        }

        /*--Exist-----------------------------------------------------------------------------------------*/

        public async Task<bool> ExistSecretsInFolder(int folderId) => await _context.Secrets.AnyAsync(s => s.IdFolder == folderId);

        public async Task<bool> Exist(int folderId) => await _context.Folders.AnyAsync(s => s.IdFolder == folderId);
    }
}
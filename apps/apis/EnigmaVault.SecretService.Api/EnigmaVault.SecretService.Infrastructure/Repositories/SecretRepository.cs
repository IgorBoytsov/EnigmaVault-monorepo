using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Features.Secrets;
using EnigmaVault.SecretService.Domain.DomainModels;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using EnigmaVault.SecretService.Infrastructure.Data;
using EnigmaVault.SecretService.Infrastructure.Data.Entities;
using EnigmaVault.SecretService.Infrastructure.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace EnigmaVault.SecretService.Infrastructure.Repositories
{
    internal class SecretRepository(SecretDBContext context, IEntityUpdater entityUpdater) : ISecretRepository
    {
        private readonly SecretDBContext _context = context;
        private readonly IEntityUpdater _entityUpdater = entityUpdater;

        /*--Create----------------------------------------------------------------------------------------*/

        public async Task<SecretDomain> CreateAsync(SecretDomain domain)
        {
            var entity = new Secret
            {
                IdUser = domain.IdUser,
                EncryptedData = domain.EncryptedData,
                Nonce = domain.Nonce,
                DateAdded = domain.DateAdded,
                DateUpdate = domain.DateUpdate,
                IsFavorite = domain.IsFavorite,
                Url = domain.Url,
                Notes = domain.Notes,
                SchemaVersion = domain.SchemaVersion,
                ServiceName = domain.ServiceName
            };

            await _context.Secrets.AddAsync(entity);
            await _context.SaveChangesAsync();
            
            var secretDomain = SecretDomain.Reconstruct(entity.IdSecret, entity.IdUser, entity.IdFolder, entity.EncryptedData, entity.Nonce, entity.ServiceName, entity.Url, entity.Notes, entity.SvgIcon, entity.SchemaVersion, entity.DateAdded, entity.DateUpdate, entity.IsFavorite, entity.IsArchive);

            return secretDomain;
        }

        /*--Get-------------------------------------------------------------------------------------------*/

        public async IAsyncEnumerable<SecretDto> GetAllStreamingAsync(int idUser, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var dtosStream = _context.Secrets
                .AsNoTracking()
                .Where(s => s.IdUser == idUser)
                .Select(s => new SecretDto
                {
                    IdUser = s.IdUser,
                    IdFolder = s.IdFolder,
                    IdSecret = s.IdSecret,
                    ServiceName = s.ServiceName,
                    Url = s.Url,
                    IsFavorite = s.IsFavorite,
                    IsArchive = s.IsArchive,
                    DateUpdate = s.DateUpdate,
                    DateAdded = s.DateAdded,
                    EncryptedData = s.EncryptedData,
                    Nonce = s.Nonce,
                    Notes = s.Notes,
                    SvgIcon = s.SvgIcon,
                    SchemaVersion = s.SchemaVersion,
                })
                .AsAsyncEnumerable()
                .WithCancellation(cancellationToken);

            await foreach (var dto in dtosStream)
                yield return dto;
        }

        public async Task<SecretDomain?> GetByIdAsync(int idSecret, CancellationToken cancellationToken)
        {
            var entity = await _context.Secrets.AsNoTracking().FirstOrDefaultAsync(s => s.IdSecret == idSecret, cancellationToken);

            if (entity is null)
                return null;

            var secretDomain = SecretDomain.Reconstruct(entity.IdSecret, entity.IdUser, entity.IdFolder, entity.EncryptedData, entity.Nonce, entity.ServiceName, entity.Url, entity.Notes, entity.SvgIcon, entity.SchemaVersion, entity.DateAdded, entity.DateUpdate, entity.IsFavorite, entity.IsArchive);

            return secretDomain;
        }

        /*--Update----------------------------------------------------------------------------------------*/

        public async Task<Result<DateTime>> UpdateAsync(SecretDomain domain)
        {
            var storage = await _context.Secrets.FirstOrDefaultAsync(s => s.IdSecret == domain.IdSecret);

            if (storage is null)
                return Result<DateTime>.Failure(new Error(ErrorCode.UpdateError, "Не найдена запись."));

            MapFromDomain(domain, storage);

            try
            {
                var countLineInUpdate = await _context.SaveChangesAsync();

                if (countLineInUpdate > 0)
                    return Result<DateTime>.Success(storage.DateUpdate);
                else
                    return Result<DateTime>.Failure(new Error(ErrorCode.UpdateError, "Не удалось изменить данные."));
            }
            catch (DbUpdateConcurrencyException)
            {
                return Result<DateTime>.Failure(new Error(ErrorCode.ConcurrencyError, "Данные были изменены другим пользователем. Пожалуйста, обновите страницу."));
            }  
        }

        /*--Delete----------------------------------------------------------------------------------------*/

        public async Task<bool> DeleteAsync(int id)
        {
            var idTiDelete = await _context.Secrets.Where(x => x.IdSecret == id).ExecuteDeleteAsync();

            if (idTiDelete > 0)
                return true;
            else
                return false;
        }

        /*--Exist-----------------------------------------------------------------------------------------*/

        public async Task<bool> ExistSecret(int idSecret) => await _context.Secrets.AnyAsync(s => s.IdSecret == idSecret);

        /*--Вспомогательные-------------------------------------------------------------------------------*/

        private static void MapFromDomain(SecretDomain secretDomain, Secret secretToUpdate)
        {
            secretToUpdate.EncryptedData = secretDomain.EncryptedData;
            secretToUpdate.Nonce= secretDomain.Nonce;
            secretToUpdate.SchemaVersion= secretDomain.SchemaVersion;

            secretToUpdate.ServiceName = secretDomain.ServiceName;
            secretToUpdate.Url = secretDomain.Url;
            secretToUpdate.Notes = secretDomain.Notes;
            secretToUpdate.SvgIcon = secretDomain.SvgIcon;

            secretToUpdate.IsFavorite = secretDomain.IsFavorite;
            secretToUpdate.IsArchive = secretDomain.IsArchive;

            secretToUpdate.IdFolder = secretDomain.IdFolder;

            secretToUpdate.DateUpdate = secretDomain.DateUpdate;
        }
    }
}
using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Features.Secrets;
using EnigmaVault.SecretService.Application.Features.Secrets.Update;
using EnigmaVault.SecretService.Domain.DomainModels;
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
            
            var secretDomain = SecretDomain.Reconstruct(entity.IdSecret, entity.IdUser, entity.IdFolder, entity.EncryptedData, entity.Nonce, entity.ServiceName, entity.Url, entity.Notes, entity.SchemaVersion, entity.DateAdded, entity.DateUpdate, entity.IsFavorite);

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
                    DateUpdate = s.DateUpdate,
                    DateAdded = s.DateAdded,
                    EncryptedData = s.EncryptedData,
                    Nonce = s.Nonce,
                    Notes = s.Notes,
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

            var secretDomain = SecretDomain.Reconstruct(entity.IdSecret, entity.IdUser, entity.IdFolder, entity.EncryptedData, entity.Nonce, entity.ServiceName, entity.Url, entity.Notes, entity.SchemaVersion, entity.DateAdded, entity.DateUpdate, entity.IsFavorite);

            return secretDomain;
        }

        /*--Update----------------------------------------------------------------------------------------*/

        public async Task<Result<DateTime>> UpdateAsync(SecretDomain secret)
            => await _entityUpdater.UpdateAndGetAsync<Secret, DateTime>(
                predicate: secretInDb => secretInDb.IdSecret == secret.IdSecret,
                setPropertyCalls: us => us
                    .SetProperty(p => p.EncryptedData, secret.EncryptedData)
                    .SetProperty(p => p.Nonce, secret.Nonce)
                    .SetProperty(p => p.ServiceName, secret.ServiceName)
                    .SetProperty(p => p.Url, secret.Url) 
                    .SetProperty(p => p.Notes, secret.Notes)
                    .SetProperty(p => p.SchemaVersion, secret.SchemaVersion)
                    .SetProperty(p => p.IsFavorite, secret.IsFavorite)
                    .SetProperty(p => p.DateUpdate, secret.DateUpdate),
                selector: secret => secret.DateUpdate);

        public async Task<Result<DateTime>> UpdateMetadataAsync(UpdateMetadataCommand command)
            => await _entityUpdater.UpdateAndGetAsync<Secret, DateTime>(
                predicate: secretInDb => secretInDb.IdSecret == command.IdSecret,
                setPropertyCalls: us => us
                    .SetProperty(p => p.ServiceName, command.ServiceName)
                    .SetProperty(p => p.Url, command.Url)
                    .SetProperty(p => p.DateUpdate, DateTime.UtcNow),
                selector: secret => secret.DateUpdate);

        public async Task<Result<DateTime>> UpdateEncryptedDataAsync(UpdateEncryptedDataCommand command)
            => await _entityUpdater.UpdateAndGetAsync<Secret, DateTime>(
                predicate: secretInDb => secretInDb.IdSecret == command.IdSecret,
                setPropertyCalls: us => us
                .SetProperty(p => p.EncryptedData, command.EncryptedData)
                .SetProperty(p => p.Nonce, command.Nonce)
                .SetProperty(p => p.SchemaVersion, command.SchemaVersion)
                .SetProperty(p => p.DateUpdate, DateTime.UtcNow),
                selector: secret => secret.DateUpdate);

        public async Task<Result<DateTime>> UpdateFavoriteAsync(UpdateFavoriteCommand command)
            => await _entityUpdater.UpdateAndGetAsync<Secret, DateTime>(
                predicate: secretInDb => secretInDb.IdSecret == command.IdSecret,
                setPropertyCalls: us => us
                .SetProperty(p => p.IsFavorite, command.IsFavorite)
                .SetProperty(p => p.DateUpdate, DateTime.UtcNow),
                selector: secret => secret.DateUpdate);

        public async Task<Result<DateTime>> UpdateNoteAsync(UpdateNoteCommand command)
            => await _entityUpdater.UpdateAndGetAsync<Secret, DateTime>(
                 predicate: secretInDb => secretInDb.IdSecret == command.IdSecret,
                 setPropertyCalls: us => us
                .SetProperty(p => p.Notes, command.Note)
                .SetProperty(p => p.DateUpdate, DateTime.UtcNow),
                selector: secret => secret.DateUpdate);

        public async Task<Result> UpdateFolderAsync(UpdateSecretFolderCommand command)
            => await _entityUpdater.UpdatePropertyAsync<Secret>(
                predicate: secretInDb => secretInDb.IdSecret == command.IdSecret,
                setPropertyCalls: us => us
                .SetProperty(p => p.IdFolder, command.IdFolder));

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
    }
}
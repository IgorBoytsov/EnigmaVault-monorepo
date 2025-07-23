using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Features.Icons;
using EnigmaVault.SecretService.Domain.DomainModels;
using EnigmaVault.SecretService.Domain.Results;
using EnigmaVault.SecretService.Infrastructure.Data;
using EnigmaVault.SecretService.Infrastructure.Data.Entities;
using EnigmaVault.SecretService.Infrastructure.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace EnigmaVault.SecretService.Infrastructure.Repositories
{
    internal sealed class IconRepository(SecretDBContext context, IEntityUpdater entityUpdater) : IIconRepository
    {
        private readonly SecretDBContext _context = context;
        private readonly IEntityUpdater _entityUpdater = entityUpdater;

        /*--Create----------------------------------------------------------------------------------------*/

        public async Task<Result<IconDomain>> CreateAsync(IconDomain icon)
        {
            var entity = new Icon
            {
                IdUser = icon.IdUser,
                SvgCode = icon.SvgCode,
                IconName = icon.IconName,
                IdIconCategory = icon.IdIconCategory,
                IsCommon = icon.IsCommon,
            };

            await _context.Icons.AddAsync(entity);
            await _context.SaveChangesAsync();

            var domain = IconDomain.Reconstruct(entity.IdIcon, entity.IdUser, entity.SvgCode, entity.IconName, entity.IdIconCategory, entity.IsCommon);

            return Result<IconDomain>.Success(domain);
        }

        /*--Get-------------------------------------------------------------------------------------------*/

        public async IAsyncEnumerable<IconDto> GetAllStreamingAsync(int? idUser, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var dtosStream = _context.Icons
                .AsNoTracking()
                .Where(i => i.IdUser == idUser || i.IsCommon == true)
                .Select(s => new IconDto(s.IdIcon, s.IdUser, s.SvgCode, s.IconName, s.IdIconCategory, s.IsCommon))
                .AsAsyncEnumerable()
                .WithCancellation(cancellationToken);

            await foreach (var dto in dtosStream)
                yield return dto;
        }

        public async Task<IconDomain?> GetByIdAsync(int idUser, int idIcon)
        {
            var entity = await _context.Icons.FirstOrDefaultAsync(i => i.IdUser == idUser && i.IdIcon == idIcon);

            if (entity is null) return null;

            var domain = IconDomain.Reconstruct(entity.IdIcon, entity.IdUser, entity.SvgCode, entity.IconName, entity.IdIconCategory, entity.IsCommon);

            return domain;
        }

        /*--Update----------------------------------------------------------------------------------------*/

        public async Task<Result> UpdateIconAsync(IconDomain icon)
            => await _entityUpdater.UpdatePropertyAsync<Icon>(
                predicate: iconInDb => iconInDb.IdIcon == icon.IdIcon && iconInDb.IdUser == icon.IdUser && iconInDb.IsCommon == false,
                setPropertyCalls: i => i
                .SetProperty(p => p.IconName, icon.IconName)
                .SetProperty(p => p.SvgCode, icon.SvgCode));
    }
}
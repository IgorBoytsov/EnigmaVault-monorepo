using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Features.IconCategories;
using EnigmaVault.SecretService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace EnigmaVault.SecretService.Infrastructure.Repositories
{
    internal sealed class IconCategoryRepository(SecretDBContext context) : IIconCategoryRepository
    {
        private readonly SecretDBContext _context = context;

        /*--Get-------------------------------------------------------------------------------------------*/

        public async IAsyncEnumerable<IconCategoryDto> GetAllStreamingAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var dtosStream = _context.IconCategories
                .AsNoTracking()
                .Select(s => new IconCategoryDto(s.IdCategory, s.Name))
                .AsAsyncEnumerable()
                .WithCancellation(cancellationToken);

            await foreach (var dto in dtosStream)
                yield return dto;
        }
    }
}
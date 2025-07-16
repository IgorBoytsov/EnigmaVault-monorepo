using EnigmaVault.SecretService.Application.Features.IconCategories;
using System.Runtime.CompilerServices;

namespace EnigmaVault.SecretService.Application.Abstractions.Repositories
{
    public interface IIconCategoryRepository
    {
        IAsyncEnumerable<IconCategoryDto> GetAllStreamingAsync([EnumeratorCancellation] CancellationToken cancellationToken = default);
    }
}
using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using MediatR;
using System.Runtime.CompilerServices;

namespace EnigmaVault.SecretService.Application.Features.IconCategories.GetAll
{
    public class GetAllIconCategoriesQueryHandler(IIconCategoryRepository iconCategoryRepository) : IStreamRequestHandler<GetAllIconCategoriesQuery, IconCategoryDto>
    {
        private readonly IIconCategoryRepository _iconCategoryRepository = iconCategoryRepository;

        public async IAsyncEnumerable<IconCategoryDto> Handle(GetAllIconCategoriesQuery request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var foldersStream = _iconCategoryRepository.GetAllStreamingAsync(cancellationToken);

            await foreach (var folder in foldersStream.WithCancellation(cancellationToken))
                yield return folder;
        }
    }
}
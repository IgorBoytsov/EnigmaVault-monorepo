using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.IconCategories.GetAll
{
    public class GetAllIconCategoriesQueryHandler(IIconCategoryRepository iconCategoryRepository) : IRequestHandler<GetAllIconCategoriesQuery, IAsyncEnumerable<IconCategoryDto>>
    {
        private readonly IIconCategoryRepository _iconCategoryRepository = iconCategoryRepository;

        public Task<IAsyncEnumerable<IconCategoryDto>> Handle(GetAllIconCategoriesQuery request, CancellationToken cancellationToken) => Task.FromResult(_iconCategoryRepository.GetAllStreamingAsync(cancellationToken));
    }
}
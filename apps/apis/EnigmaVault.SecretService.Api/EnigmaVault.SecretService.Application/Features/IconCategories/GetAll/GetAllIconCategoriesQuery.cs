using MediatR;

namespace EnigmaVault.SecretService.Application.Features.IconCategories.GetAll
{
    public record GetAllIconCategoriesQuery() : IStreamRequest<IconCategoryDto>
    {
    }
}
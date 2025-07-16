using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos;
using EnigmaVault.Application.UseCases.Abstractions.IconCategory;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Implementations.IconCategory
{
    public class GetAllIconCategoryUseCase(IIconCategoryRepository apiIconCategoryRepository) : IGetAllIconCategoryUseCase
    {
        private readonly IIconCategoryRepository _apiIconCategoryRepository = apiIconCategoryRepository;

        public async Task<Result<List<IconCategoryDto>?>> GetAllAsync() => await _apiIconCategoryRepository.GetAllAsync();
    }
}
using EnigmaVault.Application.Dtos;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.IconCategory
{
    public interface IGetAllIconCategoryUseCase
    {
        Task<Result<List<IconCategoryDto>?>> GetAllAsync();
    }
}
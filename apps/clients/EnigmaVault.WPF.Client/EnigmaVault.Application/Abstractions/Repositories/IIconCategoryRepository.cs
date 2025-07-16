using EnigmaVault.Application.Dtos;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.Abstractions.Repositories
{
    public interface IIconCategoryRepository
    {
        Task<Result<List<IconCategoryDto>?>> GetAllAsync();
    }
}
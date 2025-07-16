using EnigmaVault.Application.Dtos;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.IconCase
{
    public interface IGetAllIconsUseCase
    {
        Task<Result<List<IconDto>?>> GetAllAsync(int userId);
    }
}
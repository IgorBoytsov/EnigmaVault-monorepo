using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos;
using EnigmaVault.Application.UseCases.Abstractions.IconCase;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Implementations.IconCase
{
    public class GetAllIconsUseCase(IIconRepository IconRepository) : IGetAllIconsUseCase
    {
        private readonly IIconRepository _iconRepository = IconRepository;

        public async Task<Result<List<IconDto>?>> GetAllAsync(int userId) => await _iconRepository.GetAllAsync(userId);
    }
}
using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos;
using EnigmaVault.Application.UseCases.Abstractions.GanderCase;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Implementations.GenderCase
{
    public class GetGendersUseCase(IGenderRepository genderRepository) : IGetGendersUseCase
    {
        private readonly IGenderRepository _genderRepository = genderRepository;

        public async Task<Result<List<GenderDto>>> GetGendersAsync() => await _genderRepository.GetGendersAsync();
    }
}
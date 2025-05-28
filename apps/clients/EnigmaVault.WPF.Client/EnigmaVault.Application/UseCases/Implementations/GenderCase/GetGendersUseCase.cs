using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos;
using EnigmaVault.Application.Mappers;
using EnigmaVault.Application.UseCases.Abstractions.GanderCase;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Implementations.GenderCase
{
    public class GetGendersUseCase(IGenderRepository genderRepository) : IGetGendersUseCase
    {
        private readonly IGenderRepository _genderRepository = genderRepository;

        public async Task<Result<List<GenderDto>?>> GetGendersAsync()
        {
            var result = await _genderRepository.GetGendersAsync();

            if (result is not null && result.IsSuccess)
                return Result<List<GenderDto>?>.Success(result.Value!.Select(g => g.ToDto()).ToList());
            else
                return Result<List<GenderDto>?>.Failure(result!.Errors.ToList());
        }
    }
}
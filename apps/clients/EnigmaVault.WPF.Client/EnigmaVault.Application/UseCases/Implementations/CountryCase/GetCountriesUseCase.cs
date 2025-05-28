using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos;
using EnigmaVault.Application.Mappers;
using EnigmaVault.Application.UseCases.Abstractions.CountryCase;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Implementations.CountryCase
{
    public class GetCountriesUseCase(ICountryRepository countryRepository) : IGetCountriesUseCase
    {
        private readonly ICountryRepository _countryRepository = countryRepository;

        public async Task<Result<List<CountryDto>?>> GetCountriesAsync()
        {
            var result = await _countryRepository.GetGendersAsync();

            if (result is not null && result.IsSuccess)
                return Result<List<CountryDto>?>.Success(result.Value!.Select(g => g.ToDto()).ToList());
            else
                return Result<List<CountryDto>?>.Failure(result!.Errors.ToList());
        }
    }
}
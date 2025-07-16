using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos;
using EnigmaVault.Application.UseCases.Abstractions.CountryCase;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Implementations.CountryCase
{
    public class GetCountriesUseCase(ICountryRepository countryRepository) : IGetCountriesUseCase
    {
        private readonly ICountryRepository _countryRepository = countryRepository;

        public async Task<Result<List<CountryDto>>> GetCountriesAsync() => await _countryRepository.GetGendersAsync();
    }
}
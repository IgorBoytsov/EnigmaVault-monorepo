using AutoMapper;
using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos;
using EnigmaVault.Application.UseCases.Abstractions.CountryCase;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Implementations.CountryCase
{
    public class GetCountriesUseCase(ICountryRepository countryRepository,
                                     IMapper mapper) : IGetCountriesUseCase
    {
        private readonly ICountryRepository _countryRepository = countryRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<List<CountryDto>?>> GetCountriesAsync()
        {
            var result = await _countryRepository.GetGendersAsync();

            if (result is not null && result.IsSuccess)
                return Result<List<CountryDto>?>.Success(result.Value!.Select(c => _mapper.Map<CountryDto>(c)).ToList());
            else
                return Result<List<CountryDto>?>.Failure(result!.Errors.ToList());
        }
    }
}
using AutoMapper;
using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos;
using EnigmaVault.Application.UseCases.Abstractions.CountryCase;
using EnigmaVault.Domain.Results;
using Microsoft.Extensions.Logging;

namespace EnigmaVault.Application.UseCases.Implementations.CountryCase
{
    public class GetCountriesUseCase(ICountryRepository countryRepository,
                                     IMapper mapper,
                                     ILogger<GetCountriesUseCase> logger) : IGetCountriesUseCase
    {
        private readonly ICountryRepository _countryRepository = countryRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetCountriesUseCase> _logger = logger;

        public async Task<Result<List<CountryDto>?>> GetCountriesAsync()
        {
            _logger.LogInformation("GetCountriesUseCase начал выполнение _countryRepository.GetGendersAsync()");

            var result = await _countryRepository.GetGendersAsync();

            if (result is not null && result.IsSuccess)
            {
                _logger.LogInformation("_countryRepository.GetGendersAsync() выполнился успешно");
                return Result<List<CountryDto>?>.Success(result.Value!.Select(c => _mapper.Map<CountryDto>(c)).ToList());
            }
            else
            {
                var errors = result!.Errors.ToList();
                _logger.LogInformation("_countryRepository.GetGendersAsync() выполнился с ошибками: {@errors}", errors);
                return Result<List<CountryDto>?>.Failure(errors);
            }
        }
    }
}
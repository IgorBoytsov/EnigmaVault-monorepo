using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Application.Abstractions.UseCases;
using EnigmaVault.AuthenticationService.Application.DTOs;
using EnigmaVault.AuthenticationService.Application.Mappers;
using System.Runtime.CompilerServices;

namespace EnigmaVault.AuthenticationService.Application.Implementations.UseCases
{
    public class GetAllCountryStreamingUseCase : IGetAllCountryStreamingUseCase
    {
        private readonly ICountryRepository _countryRepository;

        public GetAllCountryStreamingUseCase(ICountryRepository countryRepository)
        {
            _countryRepository = countryRepository;
        }

        public async IAsyncEnumerable<CountryDto> GetAllStreamingAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var countriesStream = _countryRepository.GetAllStreamingAsync(cancellationToken);

            await foreach (var country in countriesStream.WithCancellation(cancellationToken))
                yield return country;
        }
    }
}
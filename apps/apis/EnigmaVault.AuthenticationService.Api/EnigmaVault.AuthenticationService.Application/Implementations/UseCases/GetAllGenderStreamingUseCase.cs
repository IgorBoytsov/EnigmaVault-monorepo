using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Application.Abstractions.UseCases;
using EnigmaVault.AuthenticationService.Application.DTOs;
using System.Runtime.CompilerServices;

namespace EnigmaVault.AuthenticationService.Application.Implementations.UseCases
{
    public class GetAllGenderStreamingUseCase : IGetAllGenderStreamingUseCase
    {
        private readonly IGenderRepository _genderRepository;

        public GetAllGenderStreamingUseCase(IGenderRepository genderRepository)
        {
            _genderRepository = genderRepository;
        }

        public async IAsyncEnumerable<GenderDto> GetAllStreamingAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var countriesStream = _genderRepository.GetAllStreamingAsync(cancellationToken);

            await foreach (var country in countriesStream.WithCancellation(cancellationToken))
                yield return country;
        }
    }
}
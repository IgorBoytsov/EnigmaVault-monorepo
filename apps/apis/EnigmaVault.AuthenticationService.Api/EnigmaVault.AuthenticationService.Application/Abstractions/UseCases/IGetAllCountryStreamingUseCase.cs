using EnigmaVault.AuthenticationService.Application.DTOs;
using System.Runtime.CompilerServices;

namespace EnigmaVault.AuthenticationService.Application.Abstractions.UseCases
{
    public interface IGetAllCountryStreamingUseCase
    {
        IAsyncEnumerable<CountryDto?> GetAllStreamingAsync([EnumeratorCancellation] CancellationToken cancellationToken = default);
    }
}
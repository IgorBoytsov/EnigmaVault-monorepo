using EnigmaVault.AuthenticationService.Application.DTOs;
using System.Runtime.CompilerServices;

namespace EnigmaVault.AuthenticationService.Application.Abstractions.Repositories
{
    public interface ICountryRepository
    {
        IAsyncEnumerable<CountryDto> GetAllStreamingAsync([EnumeratorCancellation] CancellationToken cancellationToken = default);
    }
}
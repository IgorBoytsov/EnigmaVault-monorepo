using EnigmaVault.AuthenticationService.Domain.DomainModels;
using System.Runtime.CompilerServices;

namespace EnigmaVault.AuthenticationService.Application.Abstractions.Repositories
{
    public interface ICountryRepository
    {
        IAsyncEnumerable<CountryDomain?> GetAllStreamingAsync([EnumeratorCancellation] CancellationToken cancellationToken = default);
    }
}
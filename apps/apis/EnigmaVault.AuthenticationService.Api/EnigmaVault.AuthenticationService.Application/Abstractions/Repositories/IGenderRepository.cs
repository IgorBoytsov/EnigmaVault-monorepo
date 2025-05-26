using EnigmaVault.AuthenticationService.Domain.DomainModels;
using System.Runtime.CompilerServices;

namespace EnigmaVault.AuthenticationService.Application.Abstractions.Repositories
{
    public interface IGenderRepository
    {
        IAsyncEnumerable<GenderDomain?> GetAllStreamingAsync([EnumeratorCancellation] CancellationToken cancellationToken = default);
    }
}
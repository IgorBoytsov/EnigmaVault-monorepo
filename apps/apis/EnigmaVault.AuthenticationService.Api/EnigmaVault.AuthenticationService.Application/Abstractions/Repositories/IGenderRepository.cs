using EnigmaVault.AuthenticationService.Application.DTOs;
using System.Runtime.CompilerServices;

namespace EnigmaVault.AuthenticationService.Application.Abstractions.Repositories
{
    public interface IGenderRepository
    {
        IAsyncEnumerable<GenderDto> GetAllStreamingAsync([EnumeratorCancellation] CancellationToken cancellationToken = default);
    }
}
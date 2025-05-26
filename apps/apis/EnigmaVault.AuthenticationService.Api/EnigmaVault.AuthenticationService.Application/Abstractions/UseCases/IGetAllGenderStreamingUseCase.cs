using EnigmaVault.AuthenticationService.Application.DTOs;
using System.Runtime.CompilerServices;

namespace EnigmaVault.AuthenticationService.Application.Abstractions.UseCases
{
    public interface IGetAllGenderStreamingUseCase
    {
        IAsyncEnumerable<GenderDto?> GetAllStreamingAsync([EnumeratorCancellation] CancellationToken cancellationToken = default);
    }
}
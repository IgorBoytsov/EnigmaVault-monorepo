using EnigmaVault.SecretService.Application.Features.Icons;
using EnigmaVault.SecretService.Domain.DomainModels;
using EnigmaVault.SecretService.Domain.Results;
using System.Runtime.CompilerServices;

namespace EnigmaVault.SecretService.Application.Abstractions.Repositories
{
    public interface IIconRepository
    {
        Task<Result<IconDomain>> CreateAsync(IconDomain icon);
        IAsyncEnumerable<IconDto> GetAllStreamingAsync(int? idUser, [EnumeratorCancellation] CancellationToken cancellationToken = default);
        Task<IconDomain?> GetByIdAsync(int idUser, int idIcon);
        Task<Result> UpdateIconAsync(IconDomain icon);
    }
}
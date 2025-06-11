using EnigmaVault.SecretService.Application.Features.Secrets;
using EnigmaVault.SecretService.Application.Features.Secrets.Update;
using EnigmaVault.SecretService.Domain.DomainModels;
using EnigmaVault.SecretService.Domain.Results;
using System.Runtime.CompilerServices;

namespace EnigmaVault.SecretService.Application.Abstractions.Repositories
{
    public interface ISecretRepository
    {
        Task<SecretDomain> CreateAsync(SecretDomain domain);
        IAsyncEnumerable<SecretDto> GetAllStreamingAsync(int idUser, [EnumeratorCancellation] CancellationToken cancellationToken = default);
        Task<SecretDomain?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<Result<DateTime>> UpdateAsync(SecretDomain secret);
        Task<Result<DateTime>> UpdateMetadataAsync(UpdateMetadataCommand command);
        Task<Result<DateTime>> UpdateEncryptedDataAsync(UpdateEncryptedDataCommand command);
        Task<Result<DateTime>> UpdateFavoriteAsync(UpdateFavoriteCommand command);
        Task<Result<DateTime>> UpdateNoteAsync(UpdateNoteCommand command);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistSecret(int idSecret);
    }
}
using EnigmaVault.Domain.DomainModels;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.Abstractions.Repositories
{
    public interface ISecretRepository
    {
        Task<Result<SecretDomain?>> CreateAsync(SecretDomain secret);
        Task<Result<List<SecretDomain>?>> GetAllAsync(int userId);
        Task<Result<int?>> DeleteAsync(int idSecret);

        Task<Result<DateTime>> UpdateAsync(SecretDomain secret);
        Task<Result<DateTime>> UpdateMetaDataAsync(int idSecret, string serviceName, string? url);
        Task<Result<DateTime>> UpdateEncryptedDataAsync(int idSecret, string encryptedData, string nonce);
        Task<Result<DateTime>> UpdateFavoriteAsync(int idSecret, bool isFavorite);
        Task<Result<DateTime>> UpdateNoteAsync(int idSecret, string? note);
        Task<Result> UpdateIconAsync(int idSecret, string svgCode);
        Task<Result> UpdateFolderAsync(int idSecret, int? idFolder);
        Task<Result> UpdateIsArchiveAsync(int idSecret, bool isArchive);
    }
}
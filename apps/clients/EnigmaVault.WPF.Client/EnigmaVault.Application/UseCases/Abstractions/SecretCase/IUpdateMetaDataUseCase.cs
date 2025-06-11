using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.SecretCase
{
    public interface IUpdateMetaDataUseCase
    {
        Task<Result<DateTime>> UpdateMetaDataAsync(int idSecret, string serviceName, string? url);
    }
}
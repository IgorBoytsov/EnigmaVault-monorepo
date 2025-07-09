using EnigmaVault.SecretService.Application.Features.Secrets;
using EnigmaVault.SecretService.Domain.DomainModels;

namespace EnigmaVault.SecretService.Application.Mappers
{
    public static class SecretsMapper
    {
        public static SecretDto ToDto(this SecretDomain domain)
        {
            return new SecretDto
            {
                IdSecret = domain.IdSecret,
                IdUser = domain.IdUser,
                IdFolder = domain.IdFolder,
                EncryptedData = domain.EncryptedData,
                Nonce = domain.Nonce,
                DateAdded = domain.DateAdded,
                DateUpdate = domain.DateUpdate,
                IsFavorite = domain.IsFavorite,
                Url = domain.Url,
                Notes = domain.Notes,
                SchemaVersion = domain.SchemaVersion,
                ServiceName = domain.ServiceName,
            };
        }
    }
}
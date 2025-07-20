namespace EnigmaVault.SecretService.Api.Dtos.Requests.Secrets
{
    public sealed record UpdateSecretRequest(
        string ServiceName,
        string? Url, 
        bool? IsFavorite,
        string? Note,
        string EncryptedData,
        string Nonce,
        int SchemaVersion);
}
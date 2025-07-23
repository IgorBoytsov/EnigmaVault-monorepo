namespace EnigmaVault.SecretService.Api.Dtos.Requests.Secrets
{
    public sealed record UpdateSecretRequest(
        string ServiceName,
        string? Url, 
        bool? IsFavorite,
        string? Note,
        byte[] EncryptedData,
        byte[] Nonce,
        int SchemaVersion);
}
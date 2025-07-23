namespace EnigmaVault.SecretService.Api.Dtos.Requests.Secrets
{
    public sealed record CreateSecretRequest(
        int IdUser,
        byte[] EncryptedData,
        byte[] Nonce,
        string ServiceName,
        string? Url,
        string? Notes,
        string? SvgIcon,
        int SchemaVersion,
        bool IsFavorite);
}
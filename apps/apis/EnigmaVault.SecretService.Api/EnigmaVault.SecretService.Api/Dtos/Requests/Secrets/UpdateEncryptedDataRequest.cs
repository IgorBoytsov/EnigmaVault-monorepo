namespace EnigmaVault.SecretService.Api.Dtos.Requests.Secrets
{
    public sealed record UpdateEncryptedDataRequest(byte[] EncryptedData, byte[] Nonce, int SchemaVersion);
}
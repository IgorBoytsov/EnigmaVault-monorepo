namespace EnigmaVault.SecretService.Application.Features.Secrets.Validators.Abstractions
{
    public interface IEncryptedDataHolder
    {
        byte[] EncryptedData { get; }
        byte[] Nonce { get; }
        int SchemaVersion { get; }
    }
}
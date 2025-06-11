namespace EnigmaVault.Infrastructure.Models.Request
{
    public class SecretRequest
    {
        public int IdUser { get; set; }

        public string EncryptedData { get; set; } = null!;

        public string Nonce { get; set; } = null!;

        public string ServiceName { get; set; } = null!;

        public string? Url { get; set; }

        public string? Notes { get; set; }

        public int SchemaVersion { get; set; }

        public bool IsFavorite { get; set; }
    }
}
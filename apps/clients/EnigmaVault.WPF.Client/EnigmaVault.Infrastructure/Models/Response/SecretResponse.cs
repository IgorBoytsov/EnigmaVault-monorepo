namespace EnigmaVault.Infrastructure.Models.Response
{
    public class SecretResponse
    {
        public int IdSecret { get; set; }

        public int? IdFolder { get; set; }

        public int IdUser { get; set; }

        public string EncryptedData { get; set; } = null!;

        public string Nonce { get; set; } = null!;

        public string ServiceName { get; set; } = null!;

        public string? Url { get; set; }

        public string? Notes { get; set; }

        public string SvgIcon { get; set; }

        public int SchemaVersion { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime DateUpdate { get; set; }

        public bool IsFavorite { get; set; }

        public bool IsArchive { get; set; }
    }
}

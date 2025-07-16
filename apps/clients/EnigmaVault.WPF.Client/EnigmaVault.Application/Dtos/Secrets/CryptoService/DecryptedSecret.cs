namespace EnigmaVault.Application.Dtos.Secrets.CryptoService
{
    public class DecryptedSecret
    {
        public int IdSecret { get; set; }

        public int IdFolder { get; set; }

        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? RecoveryKeys { get; set; }

        public string? SecretWord { get; set; }

        public string ServiceName { get; set; }

        public string? Url { get; set; }

        public string SvgIcon { get; set; }

        public string? Notes { get; set; }

        public int SchemaVersion { get; set; }

        public DateTime? DateAdded { get; set; }

        public DateTime? DateUpdate { get; set; }

        public bool IsFavorite { get; set; }
    }
}
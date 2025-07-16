namespace EnigmaVault.Application.Dtos.Secrets.CryptoService
{
    public class SecretMetadata
    {
        public string ServiceName { get; set; } = null!;
        public string? Url { get; set; }
        public string? Notes { get; set; }
        public string SvgIcon { get; set; }
        public bool IsFavorite { get; set; }
    }
}
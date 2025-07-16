namespace EnigmaVault.Application.Dtos.Secrets.CryptoService
{
    public class EncryptedSecret
    {
        public int IdSecret { get; set; }
        public int? IdFolder { get; set; }
        public int IdUser { get; set; }

        public string ServiceName { get; set; }
        public string Url { get; set; }
        public string Notes { get; set; }
        public int SchemaVersion { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateUpdate { get; set; }
        public bool IsFavorite { get; set; }

        public string EncryptedData { get; set; }
        public string Nonce { get; set; }
        public string SvgIcon { get; set; }
    }
}
namespace EnigmaVault.Domain.DomainModels
{
    public class SecretDomain
    {
        private SecretDomain()
        {
            
        }

        public int IdSecret { get; private set; }

        public int IdUser { get; private set; }

        public string EncryptedData { get; private set; }

        public string Nonce { get; private set; }

        public string ServiceName { get; private set; }

        public string? Url { get; private set; }

        public string? Notes { get; private set; }

        public int SchemaVersion { get; private set; }

        public DateTime? DateAdded { get; set; }

        public DateTime? DateUpdate { get; set; }

        public bool IsFavorite { get; private set; }

        public static SecretDomain Create(int idUser, string encryptedData, string nonce, string serviceName, string? url, string? notes, int schemaVersion, bool isFavorite)
        {
            return new SecretDomain()
            {
                IdUser = idUser,
                EncryptedData = encryptedData,
                Nonce = nonce,
                ServiceName = serviceName,
                Url = url,
                Notes = notes,
                SchemaVersion = schemaVersion,
                IsFavorite = isFavorite
            };
        }

        public static SecretDomain Reconstruct(int idSecret, string encryptedData, string nonce, string serviceName, string? url, string? notes, int schemaVersion, DateTime? dateAdded, DateTime? dateUpdate, bool isFavorite)
        {
            return new SecretDomain()
            {
                IdSecret = idSecret,
                EncryptedData = encryptedData,
                Nonce = nonce,
                ServiceName = serviceName,
                Url = url,
                Notes = notes,
                SchemaVersion = schemaVersion,
                DateAdded = dateAdded,
                DateUpdate = dateUpdate,
                IsFavorite = isFavorite
            };
        }
    }
}
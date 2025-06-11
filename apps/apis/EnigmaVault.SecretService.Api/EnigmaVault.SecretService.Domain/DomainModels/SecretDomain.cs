namespace EnigmaVault.SecretService.Domain.DomainModels
{
    public class SecretDomain
    {
        private SecretDomain()
        {

        }

        public int IdSecret { get; private set; }

        public int IdUser { get; private set; }

        public byte[] EncryptedData { get; private set; } = null!;

        public byte[] Nonce { get; private set; } = null!;

        public string ServiceName { get; private set; } = null!;

        public string? Url { get; private set; }

        public string? Notes { get; private set; }

        public int SchemaVersion { get; private set; }

        public DateTime DateAdded { get; private set; }

        public DateTime DateUpdate { get; private set; }

        public bool IsFavorite { get; private set; }

        public static SecretDomain Create(int idUser, byte[] encryptedData, byte[] iv, string serviceName, string? url, string? notes, int schemaVersion)
        {
            DateTime dateAdded = DateTime.UtcNow;
            DateTime dateUpdate = DateTime.UtcNow;
            bool IsFavorite = false;

            return new SecretDomain
            {
                IdUser = idUser,
                EncryptedData = encryptedData,
                Nonce = iv,
                ServiceName = serviceName,
                Url = url,
                Notes = notes,
                SchemaVersion = schemaVersion,
                DateAdded = dateAdded,
                DateUpdate = dateUpdate,
                IsFavorite = IsFavorite
            };
        }

        public static SecretDomain Reconstruct(int idSecret, int idUser, byte[] encryptedData, byte[] iv, string serviceName, string? url, string? notes, int schemaVersion, DateTime dateAdded, DateTime dateUpdate, bool IsFavorite)
        {
            return new SecretDomain
            {
                IdSecret = idSecret,
                IdUser = idUser,
                EncryptedData = encryptedData,
                Nonce = iv,
                ServiceName = serviceName,
                Url = url,
                Notes = notes,
                SchemaVersion = schemaVersion,
                DateAdded = dateAdded,
                DateUpdate = dateUpdate,
                IsFavorite = IsFavorite
            };
        }

        #region Методы обновления

        public void UpdateEncryptedPayload(byte[] encryptedData, byte[] nonce, int schemaVersion)
        {
            ArgumentNullException.ThrowIfNull(encryptedData);
            ArgumentNullException.ThrowIfNull(nonce);

            EncryptedData = encryptedData;
            Nonce = nonce;
            SchemaVersion = schemaVersion;

            UpdateDate();
        }

        public void UpdateMetadata(string serviceName, string? url)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentException("Название сервиса не может быть пустым.", nameof(serviceName));

            if (ServiceName != serviceName || Url != url)
            {
                ServiceName = serviceName;
                UpdateDate();
            } 
            
            if ( Url != url)
            {
                Url = url;
                UpdateDate();
            }
        }

        public void UpdateNote(string? newNote)
        {
            if (Notes != newNote)
            {
                Notes = newNote;
                UpdateDate();
            }
        }

        public void UpdateFavoriteStatus(bool isFavorite)
        {
            if (IsFavorite != isFavorite)
            {
                IsFavorite = isFavorite;
                UpdateDate();
            }
        }

        #endregion

        private void UpdateDate() => DateUpdate = DateTime.UtcNow;
    }
}
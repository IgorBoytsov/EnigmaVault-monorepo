namespace EnigmaVault.SecretService.Api.Dtos.Requests.Secrets
{
    public class UpdateSecretRequest
    {
        // UpdateMetadata
        public string? ServiceName { get; set; }
        public string? Url { get; set; }

        // UpdateFavorite
        public bool? IsFavorite { get; set; }

        // UpdateNoteRequest
        public string? Note { get; set; }

        // UpdateEncryptedData
        public string? EncryptedData { get; set; }
        public string? Nonce { get; set; }
        public int? SchemaVersion { get; set; }
    }
}
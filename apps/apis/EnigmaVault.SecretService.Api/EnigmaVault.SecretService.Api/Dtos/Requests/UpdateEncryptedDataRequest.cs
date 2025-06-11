using System.ComponentModel.DataAnnotations;

namespace EnigmaVault.SecretService.Api.Dtos.Requests
{
    public class UpdateEncryptedDataRequest
    {
        [Required]
        public byte[] EncryptedData { get; set; } = null!;

        [Required]
        public byte[] Nonce { get; set; } = null!;

        [Required]
        public int SchemaVersion { get; set; }
    }
}
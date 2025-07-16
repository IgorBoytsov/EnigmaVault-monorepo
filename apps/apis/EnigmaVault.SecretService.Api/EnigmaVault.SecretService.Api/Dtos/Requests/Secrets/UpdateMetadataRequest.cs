using System.ComponentModel.DataAnnotations;

namespace EnigmaVault.SecretService.Api.Dtos.Requests.Secrets
{
    public class UpdateMetadataRequest
    {
        [Required]
        public string ServiceName { get; set; } = null!;

        [Required]
        public string? Url { get; set; }
    }
}

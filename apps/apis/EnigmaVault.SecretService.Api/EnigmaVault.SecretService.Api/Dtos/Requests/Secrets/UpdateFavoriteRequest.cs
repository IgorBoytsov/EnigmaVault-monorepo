using System.ComponentModel.DataAnnotations;

namespace EnigmaVault.SecretService.Api.Dtos.Requests.Secrets
{
    public class UpdateFavoriteRequest
    {
        [Required]
        public bool IsFavorite { get; set; }
    }
}
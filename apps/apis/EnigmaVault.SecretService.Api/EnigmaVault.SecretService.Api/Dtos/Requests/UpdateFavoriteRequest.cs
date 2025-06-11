using System.ComponentModel.DataAnnotations;

namespace EnigmaVault.SecretService.Api.Dtos.Requests
{
    public class UpdateFavoriteRequest
    {
        [Required]
        public bool IsFavorite { get; set; }
    }
}
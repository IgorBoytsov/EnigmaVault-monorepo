using System.ComponentModel.DataAnnotations;

namespace EnigmaVault.SecretService.Api.Dtos.Requests.Folders
{
    public class UpdateFolderRequest
    {
        [Required]
        public string Name { get; set; } = null!;
    }
}
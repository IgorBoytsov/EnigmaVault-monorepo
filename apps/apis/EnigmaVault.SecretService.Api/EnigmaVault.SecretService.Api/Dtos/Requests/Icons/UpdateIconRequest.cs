using System.ComponentModel.DataAnnotations;

namespace EnigmaVault.SecretService.Api.Dtos.Requests.Icons
{
    public class UpdateIconRequest
    {
        [Required]
        public int IdUser { get; set; }

        [Required]
        public string IconName { get; set; } = null!;

        [Required]
        public string SvgCode {  get; set; } = null!;
    }
}
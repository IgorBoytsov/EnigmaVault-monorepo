using System.ComponentModel.DataAnnotations;

namespace EnigmaVault.SecretService.Api.Dtos.Requests.Secrets
{
    public class UpdateNoteRequest
    {
        public string? Note { get; set; }
    }
}

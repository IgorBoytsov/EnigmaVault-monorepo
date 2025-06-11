using System.ComponentModel.DataAnnotations;

namespace EnigmaVault.SecretService.Api.Dtos.Requests
{
    public class UpdateNoteRequest
    {
        public string? Note { get; set; }
    }
}

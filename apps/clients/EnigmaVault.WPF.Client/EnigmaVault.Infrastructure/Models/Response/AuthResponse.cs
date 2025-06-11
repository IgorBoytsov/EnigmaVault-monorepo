using EnigmaVault.Application.Dtos.Secrets.CryptoService;
using System.Text.Json.Serialization;

namespace EnigmaVault.Infrastructure.Models.Response
{
    internal class AuthResponse
    {
        [JsonPropertyName("idUser")]
        public int IdUser { get; set; }

        [JsonPropertyName("login")]
        public string Login { get; set; } = null!;

        [JsonPropertyName("userName")]
        public string UserName { get; set; } = null!;

        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("idGender")]
        public int IdGender { get; set; }

        [JsonPropertyName("idCountry")]
        public int IdCountry { get; set; }

        [JsonPropertyName("cryptoParameters")]
        public CryptoParameters CryptoParameters { get; set; } = null!;
    }
}
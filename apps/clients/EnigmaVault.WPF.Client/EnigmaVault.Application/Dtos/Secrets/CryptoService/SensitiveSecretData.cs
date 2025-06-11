namespace EnigmaVault.Application.Dtos.Secrets.CryptoService
{
    public class SensitiveSecretData
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? RecoveryKeys { get; set; }

        public string? SecretWord { get; set; }
    }
}
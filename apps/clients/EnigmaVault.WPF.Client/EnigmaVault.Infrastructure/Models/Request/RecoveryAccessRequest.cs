namespace EnigmaVault.Infrastructure.Models.Request
{
    internal class RecoveryAccessRequest
    {
        public string Login { get; set; }

        public string Email { get; set; }

        public string NewPassword { get; set; }
    }
}
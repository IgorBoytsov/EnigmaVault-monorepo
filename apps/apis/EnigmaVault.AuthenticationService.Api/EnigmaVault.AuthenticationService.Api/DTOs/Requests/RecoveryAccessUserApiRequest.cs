namespace EnigmaVault.AuthenticationService.Api.DTOs.Requests
{
    public class RecoveryAccessUserApiRequest
    {
        public string Login { get; set; }

        public string Email { get; set; }

        public string NewPassword { get; set; }
    }
}
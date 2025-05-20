namespace EnigmaVault.AuthenticationService.Api.DTOs.Requests
{
    public class AuthenticateUserApiRequest
    {
        public string Login { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
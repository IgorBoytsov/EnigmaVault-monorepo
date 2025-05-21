namespace EnigmaVault.AuthenticationService.Application.DTOs.Commands
{
    public class RecoveryAccessUserCommand
    {
        public string Login {  get; set; }
        public string Email {  get; set; }
        public string NewPassword {  get; set; }
    }
}
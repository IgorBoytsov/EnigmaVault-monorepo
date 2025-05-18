namespace EnigmaVault.AuthenticationService.Application.DTOs
{
    public class UserDto
    {
        public int IdUser { get; set; }

        public string Login { get; set; } = null!;

        public string UserName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Phone { get; set; }

        public int? IdGender { get; set; }

        public int? IdCountry { get; set; }
    }
}
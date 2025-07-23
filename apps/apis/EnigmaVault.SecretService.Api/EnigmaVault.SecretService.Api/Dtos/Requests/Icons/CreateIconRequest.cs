namespace EnigmaVault.SecretService.Api.Dtos.Requests.Icons
{
    public record CreateIconRequest(int? IdUser, string SvgCode, string IconName);
}
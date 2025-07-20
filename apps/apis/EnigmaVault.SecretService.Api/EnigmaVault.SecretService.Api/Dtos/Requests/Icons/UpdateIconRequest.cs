namespace EnigmaVault.SecretService.Api.Dtos.Requests.Icons
{
    public sealed record UpdateIconRequest(int IdUser, string IconName, string SvgCode);
}
namespace EnigmaVault.Infrastructure.Models.Response
{
    public record IconResponse(int IdIcon, int? IdUser, string SvgCode, string IconName, int IdIconCategory, bool IsCommon)
    {
    }
}
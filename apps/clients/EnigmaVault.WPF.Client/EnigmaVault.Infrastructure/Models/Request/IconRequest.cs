namespace EnigmaVault.Infrastructure.Models.Request
{
    public record IconRequest(int? IdUser, string SvgCode, bool IsCommon)
    {
    }
}
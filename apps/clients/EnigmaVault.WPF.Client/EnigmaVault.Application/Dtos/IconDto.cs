namespace EnigmaVault.Application.Dtos
{
    public sealed record IconDto(int IdIcon, int? IdUser, string SvgCode, string IconName, int IdIconCategory, bool IsCommon)
    {
    }
}
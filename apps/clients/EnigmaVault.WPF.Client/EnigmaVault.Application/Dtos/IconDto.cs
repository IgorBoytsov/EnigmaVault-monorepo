namespace EnigmaVault.Application.Dtos
{
    public record IconDto(int IdIcon, int? IdUser, string SvgCode, string IconName, int IdIconCategory, bool IsCommon)
    {
    }
}
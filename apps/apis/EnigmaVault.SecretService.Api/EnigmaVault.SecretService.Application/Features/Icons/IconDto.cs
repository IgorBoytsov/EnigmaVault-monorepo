namespace EnigmaVault.SecretService.Application.Features.Icons
{
    public record IconDto(int IdIcon, int? IdUser, string SvgCode, string IconName, int IdIconCategory, bool IsCommon)
    {
    }
}
using EnigmaVault.SecretService.Application.Features.Icons;
using EnigmaVault.SecretService.Domain.DomainModels;

namespace EnigmaVault.SecretService.Application.Mappers
{
    public static class IconsMapper
    {
        public static IconDto ToDto(this IconDomain domain) => new IconDto(domain.IdIcon, domain.IdUser, domain.SvgCode, domain.IconName, domain.IdIconCategory, domain.IsCommon);
    }
}
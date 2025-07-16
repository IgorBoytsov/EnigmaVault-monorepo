namespace EnigmaVault.Domain.DomainModels
{
    public class IconDomain
    {
        private IconDomain()
        {

        }

        public int IdIcon { get; private set; }

        public int? IdUser { get; private set; }

        public string SvgCode { get; private set; } = null!;

        public string IconName { get; private set; } = null!;

        public bool IsCommon { get; private set; }

        public int IdIconCategory { get; private set; }

        public static IconDomain Create(int? idUser, string svgCode, string iconName, bool isCommon)
        {
            return new IconDomain()
            {
                IdUser = idUser,
                SvgCode = svgCode,
                IconName = iconName,
                IsCommon = isCommon
            };
        }

        public static IconDomain Reconstruct(int idIcon, int? idUser, string svgCode, string iconName, int idIconCategory, bool isCommon)
        {
            return new IconDomain()
            {
                IdIcon = idIcon,
                IdUser = idUser,
                SvgCode = svgCode,
                IconName = iconName,
                IdIconCategory = idIconCategory,
                IsCommon = isCommon
            };
        }
    }
}
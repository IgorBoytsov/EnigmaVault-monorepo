using EnigmaVault.SecretService.Domain.Constants;
using EnigmaVault.SecretService.Domain.Exceptions.Icons;

namespace EnigmaVault.SecretService.Domain.DomainModels
{
    public sealed class IconDomain
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idUser"></param>
        /// <param name="svgCode"></param>
        /// <param name="iconName"></param>
        /// <param name="idIconCategory"></param>
        /// <param name="isCommon"></param>
        /// <returns></returns>
        /// <exception cref="IconValidationException">Исключение выбрасывается когда не проходит валидация для имени иконки</exception>
        public static IconDomain Create(int? idUser, string svgCode, string iconName, bool isCommon)
        {
            Guard.Against(string.IsNullOrWhiteSpace(svgCode), () => new IconValidationException("Svg код не может быть пустым."));
            Guard.Against(string.IsNullOrWhiteSpace(iconName), () => new IconValidationException("У иконки обязано быть название. Уникальность названия не имеет значения."));

            return new IconDomain()
            {
                IdUser = idUser,
                SvgCode = svgCode,
                IconName = iconName,
                IdIconCategory = IconCategoryConstants.PERSONAL_ICON_CATEGORY_ID,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="svgCode"></param>
        /// <exception cref="IconValidationException">Исключение выбрасывается когда не проходит валидация для имени иконки</exception>
        public void UpdateSvgCode(string svgCode)
        {
            Guard.Against(string.IsNullOrWhiteSpace(svgCode), () => new IconValidationException("Svg код не может быть пустым."));

            SvgCode = svgCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="IconValidationException">Исключение выбрасывается когда не проходит валидация для имени иконки</exception>
        /// <exception cref="IconUpdateNotAllowedException">Исключение выбрасывается когда иконка является общей (Т.е для всех пользователей) в следствие чего клиент не должен иметь возможность ее менять.</exception>
        public void UpdateName(string name)
        {
            Guard.Against(string.IsNullOrWhiteSpace(name), () => new IconValidationException("У иконки обязано быть название. Уникальность названия не имеет значения."));
            Guard.Against(!CanBeUpdated(), () => new IconUpdateNotAllowedException("Нельзя обновить название у общей иконки."));

            IconName = name;
        }

        public bool CanBeUpdated() => IsCommon == false;

        public void UpdateIconCategory(int id) => IdIconCategory = id;

    }
}
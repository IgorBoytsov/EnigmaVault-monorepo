using EnigmaVault.SecretService.Application.Features.Icons.Validators.Abstractions;
using EnigmaVault.SecretService.Domain.Constants;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Icons.Validators.Implementations
{
    public sealed class IconNameValidator : AbstractValidator<IIconNameValidator>
    {
        public IconNameValidator()
        {
            RuleFor(x => x.IconName)
                .NotEmpty().WithMessage("Без названия иконки нельзя.")
                .Length(IconConstants.MIN_ICON_LENGTH, IconConstants.MAX_ICON_LENGTH).WithMessage($"Допустимая длина названия от {IconConstants.MIN_ICON_LENGTH} до {IconConstants.MAX_ICON_LENGTH}");
        }
    }
}
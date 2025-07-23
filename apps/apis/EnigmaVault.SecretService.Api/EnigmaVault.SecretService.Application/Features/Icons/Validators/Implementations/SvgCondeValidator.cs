using EnigmaVault.SecretService.Application.Features.Icons.Validators.Abstractions;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Icons.Validators.Implementations
{
    public sealed class SvgCondeValidator : AbstractValidator<ISvgCodeValidator>
    {
        public SvgCondeValidator()
        {
            RuleFor(x => x.SvgCode)
                .NotEmpty().WithMessage("SVG код не должен быть пустым.");
        }
    }
}
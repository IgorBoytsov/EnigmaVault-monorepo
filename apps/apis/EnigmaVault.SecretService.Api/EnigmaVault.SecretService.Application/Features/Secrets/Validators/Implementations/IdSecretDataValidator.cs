using EnigmaVault.SecretService.Application.Features.Secrets.Validators.Abstractions;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Validators.Implementations
{
    public sealed class IdSecretDataValidator : AbstractValidator<IIdSecretDataHolder>
    {
        public IdSecretDataValidator()
        {
            RuleFor(x => x.IdSecret)
                .NotEmpty()
                .GreaterThan(0).WithMessage("ID секрета должно быть положительным.");
        }
    }
}
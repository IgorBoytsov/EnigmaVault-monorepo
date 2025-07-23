using EnigmaVault.SecretService.Application.Features.Secrets.Validators.Abstractions;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Validators.Implementations
{
    public sealed class EncryptedDataValidator : AbstractValidator<IEncryptedDataHolder>
    {
        public EncryptedDataValidator()
        {
            RuleFor(x => x.EncryptedData)
                .NotNull().WithMessage("Необходимо предоставить зашифрованные данные.")
                .NotEmpty().WithMessage("Зашифрованные данные не могут быть пустыми.");

            RuleFor(x => x.Nonce)
                .NotNull().WithMessage("Требуется криптографический вектор (nonce).")
                .NotEmpty().WithMessage("Криптографический вектор (nonce) не может быть пустым.");

            RuleFor(x => x.SchemaVersion)
                .GreaterThan(0).WithMessage("Версия схема не может быть меньше 0");
        }
    }
}
using EnigmaVault.SecretService.Application.Features.Secrets.Validators.Implementations;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Create
{
    public sealed class CreateSecretCommandValidator : AbstractValidator<CreateSecretCommand>
    {
        public CreateSecretCommandValidator()
        {
            Include(new EncryptedDataValidator());
            Include(new ServiceNameDataValidator());
        }
    }
}
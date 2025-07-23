using EnigmaVault.SecretService.Application.Features.Secrets.Validators.Implementations;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed class UpdateSecretCommandValidator : AbstractValidator<UpdateSecretCommand>
    {
        public UpdateSecretCommandValidator()
        {
            Include(new IdSecretDataValidator());
            Include(new EncryptedDataValidator());
        }
    }
}
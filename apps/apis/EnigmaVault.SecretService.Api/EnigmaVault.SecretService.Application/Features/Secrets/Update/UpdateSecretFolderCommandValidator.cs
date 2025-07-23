using EnigmaVault.SecretService.Application.Features.Secrets.Validators.Implementations;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed class UpdateSecretFolderCommandValidator : AbstractValidator<UpdateSecretFolderCommand>
    {
        public UpdateSecretFolderCommandValidator()
        {
            Include(new IdSecretDataValidator());
        }
    }
}
using EnigmaVault.SecretService.Application.Features.Secrets.Validators.Implementations;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed class UpdateEncryptedDataCommandValidator : AbstractValidator<UpdateEncryptedDataCommand>
    {
        public UpdateEncryptedDataCommandValidator()
        {
            Include(new EncryptedDataValidator());
            Include(new IdSecretDataValidator());
        }
    }
}
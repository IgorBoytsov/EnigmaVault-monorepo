using EnigmaVault.SecretService.Application.Features.Secrets.Validators.Implementations;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Delete
{
    public sealed class DeleteSecretCommandValidator : AbstractValidator<DeleteSecretCommand>
    {
        public DeleteSecretCommandValidator()
        {
            Include(new IdSecretDataValidator());
        }
    }
}
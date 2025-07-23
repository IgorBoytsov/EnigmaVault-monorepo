using EnigmaVault.SecretService.Application.Features.Secrets.Validators.Implementations;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed class UpdateIsArchiveCommandValidator : AbstractValidator<UpdateIsArchiveCommand>
    {
        public UpdateIsArchiveCommandValidator()
        {
            Include(new IdSecretDataValidator());
        }
    }
}
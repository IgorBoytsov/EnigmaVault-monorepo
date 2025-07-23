using EnigmaVault.SecretService.Application.Features.Secrets.Validators.Implementations;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed class UpdateNoteCommandValidator : AbstractValidator<UpdateNoteCommand>
    {
        public UpdateNoteCommandValidator()
        {
            Include(new IdSecretDataValidator());
        }
    }
}
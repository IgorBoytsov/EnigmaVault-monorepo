using EnigmaVault.SecretService.Application.Features.Secrets.Validators.Implementations;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed class UpdateSvgIconCommandValidator : AbstractValidator<UpdateSvgIconCommand>
    {
        public UpdateSvgIconCommandValidator()
        {
            Include(new IdSecretDataValidator());
        }
    }
}
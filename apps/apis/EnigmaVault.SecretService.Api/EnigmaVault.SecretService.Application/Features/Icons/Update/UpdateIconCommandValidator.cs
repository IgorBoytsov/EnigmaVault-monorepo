using EnigmaVault.SecretService.Application.Features.Icons.Validators.Implementations;
using EnigmaVault.SecretService.Application.Features.SharedValidator.Implementations;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Icons.Update
{
    public sealed class UpdateIconCommandValidator : AbstractValidator<UpdateIconCommand>
    {
        public UpdateIconCommandValidator()
        {
            Include(new MustHaveUserValidator());
            Include(new IconNameValidator());
            Include(new SvgCondeValidator());
        }
    }
}
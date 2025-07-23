using EnigmaVault.SecretService.Application.Features.Icons.Validators.Implementations;
using EnigmaVault.SecretService.Application.Features.SharedValidator.Implementations;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Icons.Create
{
    public sealed class CreateIconCommandValidator : AbstractValidator<CreateIconCommand>
    {
        public CreateIconCommandValidator()
        {
            Include(new MayHaveUserValidator());
            Include(new IconNameValidator());
            Include(new SvgCondeValidator());
        }
    }
}
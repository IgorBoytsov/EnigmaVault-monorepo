using EnigmaVault.SecretService.Application.Features.Folders.Validators.Abstractions;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Folders.Validators.Implementations
{
    public sealed class IdFolderValidator : AbstractValidator<IIdFolderValidator>
    {
        public IdFolderValidator()
        {
            RuleFor(x => x.IdFolder)
                .GreaterThan(0).WithMessage("ID папки не может быть меньше 0");
        }
    }
}
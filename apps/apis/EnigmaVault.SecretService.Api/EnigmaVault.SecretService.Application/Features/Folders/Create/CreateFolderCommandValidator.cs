using EnigmaVault.SecretService.Application.Features.Folders.Validators.Implementations;
using EnigmaVault.SecretService.Application.Features.SharedValidator.Implementations;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Folders.Create
{
    public sealed class CreateFolderCommandValidator : AbstractValidator<CreateFolderCommand>
    {
        public CreateFolderCommandValidator()
        {
            Include(new MustHaveUserValidator());
            Include(new FolderNameValidator());
        }
    }
}
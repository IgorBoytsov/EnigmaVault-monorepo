using EnigmaVault.SecretService.Application.Features.Folders.Validators.Abstractions;
using EnigmaVault.SecretService.Domain.Constants;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Folders.Validators.Implementations
{
    public sealed class FolderNameValidator : AbstractValidator<IFolderNameValidator>
    {
        public FolderNameValidator()
        {
            RuleFor(x => x.FolderName)
                .NotEmpty().WithMessage("Название папки не может быть пустым.")
                .Length(FolderConstants.MIN_FOLDERNAME_LENGTH, FolderConstants.MAX_FOLDERNAME_LENGTH).WithMessage($"Минимально допустимая длинна - {FolderConstants.MIN_FOLDERNAME_LENGTH}, а максимально допустимая длина - {FolderConstants.MAX_FOLDERNAME_LENGTH}");
        }
    }
}
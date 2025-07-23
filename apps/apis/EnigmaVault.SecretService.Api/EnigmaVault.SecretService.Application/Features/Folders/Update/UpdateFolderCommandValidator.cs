using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Features.Folders.Validators.Implementations;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Folders.Update
{
    public sealed class UpdateFolderCommandValidator : AbstractValidator<UpdateFolderCommand>
    {
        private readonly IFolderRepository _folderRepository;

        public UpdateFolderCommandValidator(IFolderRepository folderRepository)
        {
            _folderRepository = folderRepository;

            Include(new FolderNameValidator());
            Include(new IdFolderValidator());

            RuleFor(x => x.IdFolder)
                .MustAsync(async (id, cancellationToken) => await _folderRepository.ExistAsync(id)).WithMessage(request => $"Папка не найдена. Возможно она удалена. Перезапустите приложение.");
        }
    }
}
using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Features.Folders.Validators.Implementations;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Folders.Delete
{
    public sealed class DeleteFolderCommandValidator : AbstractValidator<DeleteFolderCommand>
    {
        private readonly IFolderRepository _folderRepository;

        public DeleteFolderCommandValidator(IFolderRepository folderRepository)
        {
            _folderRepository = folderRepository;

            Include(new IdFolderValidator());

            RuleFor(x => x.IdFolder)
                .MustAsync(async (id, cancelToken) => await _folderRepository.ExistAsync(id)).WithMessage(request => $"Папка не найдена. Возможно она уже была удалена. Перезапустите приложение."); ;
        }
    }
}
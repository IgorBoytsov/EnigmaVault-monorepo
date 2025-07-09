using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Folders.Delete
{
    public class DeleteFolderCommandHandler(IFolderRepository folderRepository) : IRequestHandler<DeleteFolderCommand, Result>
    {
        private readonly IFolderRepository _folderRepository = folderRepository;

        public async Task<Result> Handle(DeleteFolderCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                return Result.Failure(new Error(ErrorCode.NullValue, $"Команда {nameof(DeleteFolderCommand)} была с значением null"));

            if (!await _folderRepository.Exist(request.FolderId))
                return Result.Failure(new Error(ErrorCode.NotFound, $"Папка не найдена."));

            var result = await _folderRepository.Delete(request);

            return result;
        }
    }
}
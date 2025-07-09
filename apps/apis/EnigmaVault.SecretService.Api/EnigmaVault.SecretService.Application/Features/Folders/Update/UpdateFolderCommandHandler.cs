using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Folders.Update
{
    public class UpdateFolderCommandHandler(IFolderRepository folderRepository) : IRequestHandler<UpdateFolderCommand, Result>
    {
        private readonly IFolderRepository _folderRepository = folderRepository;

        public async Task<Result> Handle(UpdateFolderCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
                return Result.Failure(new Error(ErrorCode.NullValue, $"Команда {nameof(UpdateFolderCommand)} была с значением null"));

            if (string.IsNullOrWhiteSpace(request.Name))
                return Result.Failure(new Error(ErrorCode.Empty, "Название папки не было указано"));

            if(!await _folderRepository.Exist(request.IdFolder))
                return Result.Failure(new Error(ErrorCode.NotFound, $"Папка с именем {request.Name} не найдена. Возможно она удалена. Перезапустите приложение."));

            var result = await _folderRepository.UpdateName(request);

            return result;
        }
    }
}
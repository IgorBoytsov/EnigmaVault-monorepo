using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Mappers;
using EnigmaVault.SecretService.Domain.DomainModels;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Folders.Create
{
    public class CreateFolderCommandHandler(IFolderRepository folderRepository) : IRequestHandler<CreateFolderCommand, Result<FolderDto>>
    {
        private readonly IFolderRepository _folderRepository = folderRepository;

        public async Task<Result<FolderDto>> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
        {
            var domain = FolderDomain.Create(request.IdUser, request.FolderName);

            var result = await _folderRepository.Create(domain);

            if (!result.IsSuccess)
                return Result<FolderDto>.Failure(result.Errors);

            return Result<FolderDto>.Success(result.Value!.ToDto());
        }
    }
}
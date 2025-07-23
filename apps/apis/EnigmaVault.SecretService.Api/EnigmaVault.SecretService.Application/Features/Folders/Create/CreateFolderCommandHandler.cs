using EnigmaVault.SecretService.Application.Abstractions.Common;
using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Helpers;
using EnigmaVault.SecretService.Application.Mappers;
using EnigmaVault.SecretService.Domain.DomainModels;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Folders.Create
{
    public sealed class CreateFolderCommandHandler(IFolderRepository folderRepository, IValidationService validator) : IRequestHandler<CreateFolderCommand, Result<FolderDto>>
    {
        private readonly IFolderRepository _folderRepository = folderRepository;
        private readonly IValidationService _validator = validator;

        public async Task<Result<FolderDto>> Handle(CreateFolderCommand request, CancellationToken cancellationToken)
        {
            if (RequestGuard.TryGetFailureResult<CreateFolderCommand, FolderDto>(request, out var nullFailureResult))
                return nullFailureResult;

            var validationResult = await _validator.ValidateAsync(request);

            if (ValidationGuard.TryGetFailureResult<FolderDto>(validationResult, out var failureResult))
                return failureResult;

            var domain = FolderDomain.Create(request.UserId, request.FolderName);

            var result = await _folderRepository.Create(domain);

            if (!result.IsSuccess)
                return Result<FolderDto>.Failure(result.Errors);

            return Result<FolderDto>.Success(result.Value!.ToDto());
        }
    }
}
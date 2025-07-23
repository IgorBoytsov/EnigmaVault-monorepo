using EnigmaVault.SecretService.Application.Abstractions.Common;
using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Helpers;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Folders.Delete
{
    public sealed class DeleteFolderCommandHandler(IFolderRepository folderRepository, IValidationService validator) : IRequestHandler<DeleteFolderCommand, Result>
    {
        private readonly IFolderRepository _folderRepository = folderRepository;
        private readonly IValidationService _validator = validator;

        public async Task<Result> Handle(DeleteFolderCommand request, CancellationToken cancellationToken)
        {
            if (RequestGuard.TryGetFailureResult<DeleteFolderCommand>(request, out var nullFailureResult))
                return nullFailureResult;

            var validationResult = await _validator.ValidateAsync(request);

            if (ValidationGuard.TryGetFailureResult(validationResult, out var failureResult))
                return failureResult;

            var result = await _folderRepository.Delete(request);

            return result;
        }
    }
}
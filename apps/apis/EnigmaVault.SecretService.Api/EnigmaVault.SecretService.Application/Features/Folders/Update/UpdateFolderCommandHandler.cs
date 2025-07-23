using EnigmaVault.SecretService.Application.Abstractions.Common;
using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Helpers;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Folders.Update
{
    public sealed class UpdateFolderCommandHandler(IFolderRepository folderRepository, IValidationService validator) : IRequestHandler<UpdateFolderCommand, Result>
    {
        private readonly IFolderRepository _folderRepository = folderRepository;
        private readonly IValidationService _validator = validator;

        public async Task<Result> Handle(UpdateFolderCommand request, CancellationToken cancellationToken)
        {
            if (RequestGuard.TryGetFailureResult<UpdateFolderCommand>(request, out var nullFailureResult))
                return nullFailureResult;

            var validationResult = await _validator.ValidateAsync(request);

            if (ValidationGuard.TryGetFailureResult(validationResult, out var failureResult))
                return failureResult;

            var result = await _folderRepository.UpdateName(request);

            return result;
        }
    }
}
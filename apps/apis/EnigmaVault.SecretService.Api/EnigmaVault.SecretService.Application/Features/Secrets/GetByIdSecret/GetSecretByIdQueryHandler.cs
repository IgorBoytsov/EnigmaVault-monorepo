using EnigmaVault.SecretService.Application.Abstractions.Common;
using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Helpers;
using EnigmaVault.SecretService.Application.Mappers;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.GetByIdSecret
{
    public sealed class GetSecretByIdQueryHandler(ISecretRepository secretRepository, IValidationService validator) : IRequestHandler<GetSecretByIdQuery, Result<SecretDto>>
    {
        private readonly ISecretRepository _secretRepository = secretRepository;
        private readonly IValidationService _validator = validator;

        public async Task<Result<SecretDto>> Handle(GetSecretByIdQuery request, CancellationToken cancellationToken)
        {
            if (RequestGuard.TryGetFailureResult<GetSecretByIdQuery, SecretDto>(request, out var nullFailureResult))
                return nullFailureResult;

            var validationResult = await _validator.ValidateAsync(request);

            if (ValidationGuard.TryGetFailureResult<SecretDto>(validationResult, out var validationFailureResult))
                return validationFailureResult;

            var secretDomain = await _secretRepository.GetByIdAsync(request.IdSecret, cancellationToken);

            if (secretDomain == null)
                return Result<SecretDto>.Failure(new Error(ErrorCode.NotFound, $"Запись с {request.IdSecret} ID, не была найдена."));

            var secretDto = secretDomain.ToDto();

            return Result<SecretDto>.Success(secretDto);
        }
    }
}
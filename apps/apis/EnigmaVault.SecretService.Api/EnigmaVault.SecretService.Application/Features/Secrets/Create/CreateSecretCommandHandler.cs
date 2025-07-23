using EnigmaVault.SecretService.Application.Abstractions.Common;
using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Helpers;
using EnigmaVault.SecretService.Application.Mappers;
using EnigmaVault.SecretService.Domain.DomainModels;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Create
{
    public sealed class CreateSecretCommandHandler(ISecretRepository secretRepository, IValidationService validator) : IRequestHandler<CreateSecretCommand, Result<SecretDto>>
    {
        private readonly ISecretRepository _secretRepository = secretRepository;
        private readonly IValidationService _validator = validator;

        public async Task<Result<SecretDto>> Handle(CreateSecretCommand request, CancellationToken cancellationToken)
        {
            if (RequestGuard.TryGetFailureResult<CreateSecretCommand, SecretDto>(request, out var nullFailureResult))
                return nullFailureResult;

            var validationResult = await _validator.ValidateAsync(request);

            if (ValidationGuard.TryGetFailureResult<SecretDto>(validationResult, out var validationFailureResult))
                return validationFailureResult;

            var secretDomain = SecretDomain.Create(request.IdUser, request.EncryptedData, request.Nonce, request.ServiceName, request.Url, request.Notes, request.SvgIcon, request.SchemaVersion);

            if (secretDomain is null)
                return Result<SecretDto>.Failure(new Error(ErrorCode.NullValue, "Доменная модель не создалась"));

            var created = await _secretRepository.CreateAsync(secretDomain);

            var secretDto = created.ToDto();

            return Result<SecretDto>.Success(secretDto);
        }
    }
}
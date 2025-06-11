using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Mappers;
using EnigmaVault.SecretService.Domain.DomainModels;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Create
{
    public class CreateSecretCommandHandler(ISecretRepository secretRepository) : IRequestHandler<CreateSecretCommand, Result<SecretDto>>
    {
        private readonly ISecretRepository _secretRepository = secretRepository;

        public async Task<Result<SecretDto>> Handle(CreateSecretCommand request, CancellationToken cancellationToken)
        {
            var secretDomain = SecretDomain.Create(request.IdUser, request.EncryptedData, request.Nonce, request.ServiceName, request.Url, request.Notes, request.SchemaVersion);

            if (secretDomain is null)
                return Result<SecretDto>.Failure(new Error(ErrorCode.NullValue, "Доменная модель не создалась"));

            var created = await _secretRepository.CreateAsync(secretDomain);

            var secretDto = created.ToDto();

            return Result<SecretDto>.Success(secretDto);
        }
    }
}
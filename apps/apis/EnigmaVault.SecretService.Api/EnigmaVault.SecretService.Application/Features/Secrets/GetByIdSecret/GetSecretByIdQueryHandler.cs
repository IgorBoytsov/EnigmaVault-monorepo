using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Mappers;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.GetByIdSecret
{
    public class GetSecretByIdQueryHandler(ISecretRepository secretRepository) : IRequestHandler<GetSecretByIdQuery, Result<SecretDto>>
    {
        private readonly ISecretRepository _secretRepository = secretRepository;

        public async Task<Result<SecretDto>> Handle(GetSecretByIdQuery request, CancellationToken cancellationToken)
        {
            var secretDomain = await _secretRepository.GetByIdAsync(request.IdSecret, cancellationToken);

            if (secretDomain == null)
                return Result<SecretDto>.Failure(new Error(ErrorCode.NotFound, $"Запись с {request.IdSecret} ID, не была найдена."));

            var secretDto = secretDomain.ToDto();

            return Result<SecretDto>.Success(secretDto);
        }
    }
}
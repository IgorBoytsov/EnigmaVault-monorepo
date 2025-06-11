using AutoMapper;
using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos.Secrets.CryptoService;
using EnigmaVault.Application.UseCases.Abstractions.SecretCase;
using EnigmaVault.Domain.Results;
using Microsoft.Extensions.Logging;

namespace EnigmaVault.Application.UseCases.Implementations.SecretCase
{
    public class GetAllSecretsUseCase(ISecretRepository secretRepository,
                                      IMapper mapper,
                                      ILogger<GetAllSecretsUseCase> logger) : IGetAllSecretsUseCase
    {
        private readonly ISecretRepository _secretRepository = secretRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<GetAllSecretsUseCase> _logger = logger;

        public async Task<Result<List<EncryptedSecret>?>> GetSecretsAsync(int userId)
        {
            _logger.LogInformation("Обращение к репозиторию для получения списка секретов");
            var result = await _secretRepository.GetAllAsync(userId);

            if (!result.IsSuccess)
            {
                _logger.LogError("Обращение к репозиторию вернуло ошибки {@Errors}", result.Errors);
                return Result<List<EncryptedSecret>?>.Failure(result.Errors.ToList());
            }

            var list = result.Value!.Select(s => _mapper.Map<EncryptedSecret>(s)).ToList();

            _logger.LogInformation("Обращение к репозиторию завершилось успехом!");
            return Result<List<EncryptedSecret>?>.Success(list);
        }
    }
}
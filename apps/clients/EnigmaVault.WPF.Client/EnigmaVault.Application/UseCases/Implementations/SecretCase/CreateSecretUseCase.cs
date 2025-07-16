using AutoMapper;
using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos.Secrets.CryptoService;
using EnigmaVault.Application.Services.Abstractions;
using EnigmaVault.Application.UseCases.Abstractions.SecretCase;
using EnigmaVault.Domain.DomainModels;
using EnigmaVault.Domain.Enums;
using EnigmaVault.Domain.Results;
using Microsoft.Extensions.Logging;

namespace EnigmaVault.Application.UseCases.Implementations.SecretCase
{
    public class CreateSecretUseCase(ISecretRepository secretRepository,
                                     IMapper mapper,
                                     IAuthorizationService authorizationService,
                                     ISecretsCryptoService secretsCryptoService,
                                     ILogger<CreateSecretUseCase> logger) : ICreateSecretUseCase
    {
        private readonly ISecretRepository _secretRepository = secretRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IAuthorizationService _authorizationService = authorizationService;
        private readonly ISecretsCryptoService _secretsCryptoService = secretsCryptoService;
        private readonly ILogger<CreateSecretUseCase> _logger = logger;

        public async Task<Result<EncryptedSecret>> Create(string serviceName, string username, string password, string email, string secretWord, string recoveryKeysSecret, string? url, string? notes, bool isFavorite)
        {
            _logger.LogDebug("Проверка валидации");
            var validationErrors = new List<Error>();

            if (string.IsNullOrWhiteSpace(serviceName))
                validationErrors.Add(new Error(ErrorCode.EmptyValue, "Вы не указали название записи."));

            if (string.IsNullOrWhiteSpace(username))
                validationErrors.Add(new Error(ErrorCode.EmptyValue, "Вы не указали поле для входа на сервис."));

            if (string.IsNullOrWhiteSpace(password))
                validationErrors.Add(new Error(ErrorCode.EmptyValue, "Вы не указали пароль для входа на сервис."));
  
            if (validationErrors.Any())
            {
                _logger.LogDebug("Проверка валидации завершилась неудачей. {@validationErrors}", validationErrors);
                return Result<EncryptedSecret>.Failure(validationErrors);
            }
                
            var sensitiveData = new SensitiveSecretData
            {
                Username = username,
                Password = password,
                Email = email,
                SecretWord = secretWord,
                RecoveryKeys = recoveryKeysSecret,
            };

            var metaData = new SecretMetadata
            {
                ServiceName = serviceName,
                Url = url,
                Notes = notes,
                IsFavorite = isFavorite
            };

            try
            {
                _logger.LogInformation("Стартовал процесс шифрование данных.");
                var encryptedSecret = _secretsCryptoService.Encrypt(sensitiveData, metaData);

                _logger.LogInformation("Стартовал процесс создание доменной модели.");
                var createdEncryptedSecretDomain = SecretDomain.Create(
                    _authorizationService.CurrentUser!.IdUser,
                    encryptedSecret.EncryptedData, encryptedSecret.Nonce,
                    encryptedSecret.ServiceName, encryptedSecret.Url, encryptedSecret.Notes, encryptedSecret.SvgIcon, encryptedSecret.SchemaVersion, encryptedSecret.IsFavorite);
                _logger.LogInformation("Создана доменная модель. {@createdEncryptedSecretDomain}", createdEncryptedSecretDomain);

                _logger.LogInformation("Запрос к репозиторию.");
                var returnedEncryptedSecretDomain = await _secretRepository.CreateAsync(createdEncryptedSecretDomain);
                _logger.LogInformation("Возвращена доменная модель. {@returnedEncryptedSecretDomain}", returnedEncryptedSecretDomain);

                var encryptedSecretDto = _mapper.Map<EncryptedSecret>(returnedEncryptedSecretDomain.Value);

                _logger.LogInformation("Процесс шифрование данных закончился успешно!");
                return Result<EncryptedSecret>.Success(encryptedSecretDto);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Ошибка в процессе шифрование записи.");
                return Result<EncryptedSecret>.Failure(new Error(ErrorCode.CreateError, "Ошибка создание записи."));
            }
        }
    }
}
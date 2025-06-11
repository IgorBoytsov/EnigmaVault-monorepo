using AutoMapper;
using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos;
using EnigmaVault.Application.Dtos.Secrets.CryptoService;
using EnigmaVault.Application.Services.Abstractions;
using EnigmaVault.Application.UseCases.Abstractions.UserCase;
using EnigmaVault.Domain.DomainModels;
using EnigmaVault.Domain.Enums;
using EnigmaVault.Domain.Results;
using Microsoft.Extensions.Logging;

namespace EnigmaVault.Application.UseCases.Implementations.UserCase
{
    public class AuthenticationUserUseCase(IUserRepository userRepository,
                                           ISecretsCryptoService secretsCryptoService,
                                           IMapper mapper,
                                           ILogger<AuthenticationUserUseCase> logger) : IAuthenticationUserUseCase
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ISecretsCryptoService _secretsCryptoService = secretsCryptoService;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<AuthenticationUserUseCase> _logger = logger;

        public async Task<Result<UserDto?>> AuthenticationAsync(string login, string password)
        {
            if (string.IsNullOrEmpty(login))
                return Result<UserDto?>.Failure(new Error(ErrorCode.EmptyValue, "Вы не заполнили поле с логином."));
            if (string.IsNullOrEmpty(password))
                return Result<UserDto?>.Failure(new Error(ErrorCode.EmptyValue, "Вы не заполнили поле с паролем."));
            
            _logger.LogInformation("AuthenticationUserUseCase начал выполнение _userRepository.AuthenticationAsync");

            Result<(UserDomain? User, CryptoParameters CryptoParameters)> authResult = await _userRepository.AuthenticationAsync(login, password);

            if (authResult is not null && authResult.IsSuccess)
            {
                _secretsCryptoService.GenerateEncryptionKey(password, authResult.Value.CryptoParameters);

                _logger.LogInformation("_userRepository.AuthenticationAsync() выполнился успешно");
                return Result<UserDto?>.Success(_mapper.Map<UserDto>(authResult.Value!.User));
            }
            else
            {
                var errors = authResult!.Errors.ToList();
                _logger.LogInformation("_userRepository.AuthenticationAsync() выполнился с ошибками: {@errors}", errors);
                return Result<UserDto?>.Failure(errors);
            }  
        }
    }
}
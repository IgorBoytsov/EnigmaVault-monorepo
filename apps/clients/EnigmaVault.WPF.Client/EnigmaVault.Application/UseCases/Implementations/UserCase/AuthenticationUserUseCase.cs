using AutoMapper;
using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos;
using EnigmaVault.Application.UseCases.Abstractions.UserCase;
using EnigmaVault.Domain.DomainModels;
using EnigmaVault.Domain.Enums;
using EnigmaVault.Domain.Results;
using Microsoft.Extensions.Logging;

namespace EnigmaVault.Application.UseCases.Implementations.UserCase
{
    public class AuthenticationUserUseCase(IUserRepository userRepository,
                                           IMapper mapper,
                                           ILogger<AuthenticationUserUseCase> logger) : IAuthenticationUserUseCase
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<AuthenticationUserUseCase> _logger = logger;

        public async Task<Result<UserDto?>> AuthenticationAsync(string login, string password)
        {
            if (string.IsNullOrEmpty(login))
                return Result<UserDto?>.Failure(new Error(ErrorCode.EmptyValue, "Вы не заполнили поле с логином."));
            if (string.IsNullOrEmpty(password))
                return Result<UserDto?>.Failure(new Error(ErrorCode.EmptyValue, "Вы не заполнили поле с паролем."));
            
            _logger.LogInformation("AuthenticationUserUseCase начал выполнение _userRepository.AuthenticationAsync");

            Result<UserDomain?> authResult = await _userRepository.AuthenticationAsync(login, password);

            if (authResult is not null && authResult.IsSuccess)
            {
                _logger.LogInformation("_userRepository.AuthenticationAsync() выполнился успешно");
                return Result<UserDto?>.Success(_mapper.Map<UserDto>(authResult.Value!));
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
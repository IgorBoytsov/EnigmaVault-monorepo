using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.Dtos;
using EnigmaVault.Application.Mappers;
using EnigmaVault.Application.UseCases.Abstractions.UserCase;
using EnigmaVault.Domain.DomainModels;
using EnigmaVault.Domain.Enums;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Implementations.UserCase
{
    public class AuthenticationUserUseCase(IUserRepository userRepository) : IAuthenticationUserUseCase
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<UserDto?>> AuthenticationAsync(string login, string password)
        {
            if (string.IsNullOrEmpty(login))
                return Result<UserDto?>.Failure(new Error(ErrorCode.EmptyValue, "Вы не заполнили поле с логином."));
            if (string.IsNullOrEmpty(password))
                return Result<UserDto?>.Failure(new Error(ErrorCode.EmptyValue, "Вы не заполнили поле с паролем."));

            Result<UserDomain?> authResult = await _userRepository.AuthenticationAsync(login, password);

            if (authResult is not null && authResult.IsSuccess)
                return Result<UserDto?>.Success(authResult.Value!.ToDto());
            else
                return Result<UserDto?>.Failure(authResult!.Errors.ToList());
        }
    }
}
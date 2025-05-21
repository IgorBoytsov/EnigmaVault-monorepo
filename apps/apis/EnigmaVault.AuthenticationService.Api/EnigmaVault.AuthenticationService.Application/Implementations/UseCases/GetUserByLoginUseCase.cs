using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Application.Abstractions.UseCases;
using EnigmaVault.AuthenticationService.Application.DTOs.Results;
using EnigmaVault.AuthenticationService.Application.Enums;
using EnigmaVault.AuthenticationService.Application.Mappers;

namespace EnigmaVault.AuthenticationService.Application.Implementations.UseCases
{
    public class GetUserByLoginUseCase : IGetUserByLoginUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetUserByLoginUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserResult> GetUserByLoginAsync(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
                return UserResult.FailureResult(ErrorCode.ValidationFailed, "Не был указан логин.");

            var gettingUserDomain = await _userRepository.GetUserByLoginAsync(login);

            if (gettingUserDomain is null)
                return UserResult.FailureResult(ErrorCode.SaveUserError, "Ошибка получения пользователя.");

            var userDto = gettingUserDomain.ToDto();

            return UserResult.SuccessResult(userDto);
        }
    }
}
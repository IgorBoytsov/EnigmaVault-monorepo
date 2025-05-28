using EnigmaVault.Application.Dtos;
using EnigmaVault.Application.Services.Abstractions;
using EnigmaVault.Application.UseCases.Abstractions.UserCase;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.Services.Implementations
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IAuthenticationUserUseCase _authenticationUserUseCase;
        private readonly IRegistrationUserUseCase _registrationUserUseCase;
        private readonly IRecoveryAccessUserUseCase _recoveryAccessUserUseCase;

        public AuthorizationService(IAuthenticationUserUseCase authenticationUserUseCase,
                                    IRegistrationUserUseCase registrationUserUseCase,
                                    IRecoveryAccessUserUseCase recoveryAccessUserUseCase)
        {
            _authenticationUserUseCase = authenticationUserUseCase;
            _registrationUserUseCase = registrationUserUseCase;
            _recoveryAccessUserUseCase = recoveryAccessUserUseCase;
        }

        public UserDto? CurrentUser { get; private set; }

        public async Task<Result<UserDto?>> AuthenticationAsync(string login, string password)
        {
            var result = await _authenticationUserUseCase.AuthenticationAsync(login, password);

            if (result.IsSuccess)
                CurrentUser = result.Value!;

            return result;
        }

        public async Task<Result<(string Login, string Password)?>> RegistrationAsync(string? login, string? password, string? userName, string? email, string? phone, int idGender, int idCountry)
            => await _registrationUserUseCase.RegistrationAsync(login!, password!, userName!, email!, phone, idGender, idCountry);
        
        public async Task<Result<(string Login, string Password)?>> RecoveryAccessAsync(string? login, string? email, string? newPassword)
            => await _recoveryAccessUserUseCase.RecoveryAccessAsync(login!, email!, newPassword!);
    }
}
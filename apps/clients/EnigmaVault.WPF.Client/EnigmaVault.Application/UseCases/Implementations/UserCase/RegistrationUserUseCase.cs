using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.UseCases.Abstractions.UserCase;
using EnigmaVault.Domain.Enums;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Implementations.UserCase
{
    public class RegistrationUserUseCase(IUserRepository userRepository) : IRegistrationUserUseCase
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<Result<(string Login, string Password)?>> RegistrationAsync(string login, string password, string userName, string email, string? phone, int idGender, int idCountry)
        {
            var validationErrors = new List<Error>();

            if (string.IsNullOrEmpty(login))
                validationErrors.Add(new Error(ErrorCode.EmptyValue, "Вы не заполнили поле с логином."));
            if (string.IsNullOrEmpty(password))
                validationErrors.Add(new Error(ErrorCode.EmptyValue, "Вы не заполнили поле с паролем."));
            if (string.IsNullOrEmpty(userName))
                validationErrors.Add(new Error(ErrorCode.EmptyValue, "Вы не заполнили поле с именем пользователя."));
            if (string.IsNullOrEmpty(email))
                validationErrors.Add(new Error(ErrorCode.EmptyValue, "Вы не заполнили поле с электронной почтой."));
            if (idGender <= 0)
                validationErrors.Add(new Error(ErrorCode.EmptyValue, "Вы не указали гендер."));
            if (idCountry <= 0)
                validationErrors.Add(new Error(ErrorCode.EmptyValue, "Вы не указали страну."));

            if (validationErrors.Any())
                return Result<ValueTuple<string, string>?>.Failure(validationErrors);

            Result<ValueTuple<string, string>?> authResult = await _userRepository.RegistrationAsync(login, password, userName, email, phone, idGender, idCountry);

            return authResult;
        }
    }
}
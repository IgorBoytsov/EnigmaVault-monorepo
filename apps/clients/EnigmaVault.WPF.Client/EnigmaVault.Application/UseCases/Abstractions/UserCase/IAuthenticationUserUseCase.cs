using EnigmaVault.Application.Dtos;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.UserCase
{
    public interface IAuthenticationUserUseCase
    {
        Task<Result<UserDto?>> AuthenticationAsync(string login, string password);
    }
}
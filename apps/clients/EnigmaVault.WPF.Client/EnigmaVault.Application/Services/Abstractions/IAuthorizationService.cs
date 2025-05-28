using EnigmaVault.Application.Dtos;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.Services.Abstractions
{
    public interface IAuthorizationService
    {
        UserDto? CurrentUser { get; }

        Task<Result<UserDto?>> AuthenticationAsync(string login, string password);
        Task<Result<(string Login, string Password)?>> RegistrationAsync(string? login, string? password, string? userName, string? email, string? phone, int idGender, int idCountry);
        Task<Result<(string Login, string Password)?>> RecoveryAccessAsync(string? login, string? email, string? newPassword);
    }
}
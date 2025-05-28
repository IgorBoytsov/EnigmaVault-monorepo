using EnigmaVault.Domain.DomainModels;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.Abstractions.Repositories
{
    public interface IUserRepository
    {
        Task<Result<UserDomain?>> AuthenticationAsync(string login, string password);
        Task<Result<(string Login, string Password)?>> RegistrationAsync(string login, string password, string userName, string email, string? phone, int idGender, int idCountry);
        Task<Result<(string Login, string NewPassword)?>> RecoveryAccessAsync(string login, string email, string newPassword);
    }
}
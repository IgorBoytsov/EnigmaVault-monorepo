using EnigmaVault.AuthenticationService.Domain.DomainModels;

namespace EnigmaVault.AuthenticationService.Application.Abstractions.Repositories
{
    public interface IUserRepository
    {
        Task<UserDomain?> CreateAsync(UserDomain userDomain);
        Task<bool> ExistsByLoginAsync(string login);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByPhoneAsync(string? phone);
    }
}
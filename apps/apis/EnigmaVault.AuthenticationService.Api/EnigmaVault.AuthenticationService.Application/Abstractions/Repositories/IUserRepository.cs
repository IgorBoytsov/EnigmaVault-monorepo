using EnigmaVault.AuthenticationService.Domain.DomainModels;

namespace EnigmaVault.AuthenticationService.Application.Abstractions.Repositories
{
    public interface IUserRepository
    {
        /*--CRUD------------------------------------------------------------------------------------------*/

        Task<UserDomain?> CreateAsync(UserDomain userDomain);

        /*--Get-------------------------------------------------------------------------------------------*/

        /// <summary>
        /// Возвращает хранимый хеш.
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        Task<string> GetHashByLoginAsync(string login);
        Task<UserDomain?> GetUserByLogin(string login);

        /*--Exist-----------------------------------------------------------------------------------------*/

        Task<bool> ExistsByLoginAsync(string login);
        Task<bool> ExistsByEmailAsync(string email);
        Task<bool> ExistsByPhoneAsync(string? phone);
    }
}
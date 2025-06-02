using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Domain.DomainModels;
using EnigmaVault.AuthenticationService.Infrastructure.Data;
using EnigmaVault.AuthenticationService.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EnigmaVault.AuthenticationService.Infrastructure.Repositories
{
    public class UserRepository(UsersDBContext context, ILogger<UserRepository> logger) : IUserRepository
    {
        private readonly UsersDBContext _context = context;
        private readonly ILogger<UserRepository> _logger = logger;

        /*--CRUD------------------------------------------------------------------------------------------*/

        public async Task<UserDomain?> CreateAsync(UserDomain userDomain)
        {
            var entity = new User
            {
                Login = userDomain.Login,
                UserName = userDomain.UserName,
                PasswordHash = userDomain.PasswordHash,
                Email = userDomain.Email,
                Phone = userDomain.Phone,
                DateRegistration = userDomain.DateRegistration,
                DateEntry = userDomain.DateEntry,
                DateUpdate = userDomain.DateUpdate,
                IdGender = userDomain.IdGender,
                IdRole = userDomain.IdRole,
                IdCountry = userDomain.IdCountry,
                IdStatusAccount = userDomain.IdStatusAccount,
            };

            _logger.LogDebug("Попытка создания пользователя в БД. Login: {Login}, Email: {Email}", entity.Login, entity.Email);

            try
            {
                await _context.Users.AddAsync(entity);
                await _context.SaveChangesAsync();

                var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Login.ToLower() == userDomain.Login.ToLower() && x.UserName.ToLower() == userDomain.UserName.ToLower());

                if (user == null) 
                {
                    _logger.LogError("Пользователь не был найден в БД. Login: {Login}", entity.Login);
                }

                _logger.LogInformation("Пользователь успешно создан в БД. Login: {Login}, UserId: {UserId}", entity.Login, entity.IdUser); 

                var domain = UserDomain.Reconstitute(
                    entity.IdUser, entity.Login, entity.UserName, entity.PasswordHash, entity.Email, entity.Phone,
                    entity.DateRegistration, entity.DateEntry, entity.DateUpdate,
                    entity.IdStatusAccount, entity.IdGender, entity.IdCountry, entity.IdRole);

                return domain;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Ошибка DbUpdateException при создании пользователя {Login} в БД.", entity.Login);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Непредвиденная ошибка при создании пользователя {Login} в БД.", entity.Login);
                return null;
            }
        }

        public async Task<bool> UpdatePasswordAsync(string login, string email, string newHash)
        {
            _logger.LogDebug("Попытка обновления пароля для Login: {Login}, Email: {Email}", login, email);

            try
            {
                var updatedRows = await _context.Users
                    .Where(u => u.Login.ToLower() == login.ToLower() && u.Email.ToLower() == email.ToLower())
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(p => p.PasswordHash, newHash)
                        .SetProperty(d => d.DateUpdate, DateTime.UtcNow));

                if (updatedRows > 0)
                {
                    _logger.LogInformation("Пароль успешно обновлен для Login: {Login}. Затронуто строк: {UpdatedRows}", login, updatedRows);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Попытка обновления пароля для Login: {Login}, Email: {Email} не затронула ни одной строки (пользователь не найден или данные не изменились).", login, email);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении пароля для Login: {Login}, Email: {Email}.", login, email);
                return false;
            }
        }

        public async Task<bool> UpdateDateEntryAsync(int id)
        {
            _logger.LogDebug("Попытка обновления DateEntry для UserId: {UserId}", id);
            try
            {
                var updatedRows = await _context.Users
                    .Where(u => u.IdUser == id)
                    .ExecuteUpdateAsync(setters => setters
                    .SetProperty(d => d.DateEntry, DateTime.UtcNow));

                if (updatedRows > 0)
                {
                    _logger.LogInformation("DateEntry успешно обновлен для UserId: {UserId}. Затронуто строк: {UpdatedRows}", id, updatedRows);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Попытка обновления DateEntry для UserId: {UserId} не затронула ни одной строки (пользователь не найден).", id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении DateEntry для UserId: {UserId}.", id);
                return false;
            }
        }

        /*--Get-------------------------------------------------------------------------------------------*/

        public async Task<string> GetHashByLoginAsync(string login)
        {
            _logger.LogDebug("Запрос хеша пароля для Login: {Login}", login);
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Login.ToLower() == login.ToLower());

            if (user is null)
            {
                _logger.LogWarning("Хеш пароля не найден для Login: {Login} (пользователь отсутствует).", login);
                throw new ArgumentNullException("Пользователь не найден.");
            }
            return user.PasswordHash;
        }

        public async Task<UserDomain?> GetUserByLoginAsync(string login)
        {
            _logger.LogDebug("Запрос пользователя по Login: {Login}", login);
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Login.ToLower() == login.ToLower());

            if (user is null)
            {
                _logger.LogDebug("Пользователь с Login: {Login} не найден в БД.", login);
                return null;
            }

            _logger.LogDebug("Пользователь с Login: {Login} найден. UserId: {UserId}", login, user.IdUser);
            return UserDomain.Reconstitute(
                user.IdUser, user.Login, user.UserName, user.PasswordHash, user.Email, user.Phone,
                user.DateRegistration, user.DateEntry, user.DateUpdate,
                user.IdStatusAccount, user.IdGender, user.IdCountry, user.IdRole);
        }

        /*--Exist-----------------------------------------------------------------------------------------*/

        public async Task<bool> ExistsByLoginAsync(string login) => await _context.Users.AsNoTracking().AnyAsync(x => x.Login.ToLower() == login.ToLower());

        public async Task<bool> ExistsByEmailAsync(string email) => await _context.Users.AsNoTracking().AnyAsync(x => x.Email.ToLower() == email.ToLower());

        public async Task<bool> ExistsByPhoneAsync(string? phone)
        {
            if (phone is null) return false;
            return await _context.Users.AsNoTracking().AnyAsync(x => x.Phone == phone);
        }
    }
}
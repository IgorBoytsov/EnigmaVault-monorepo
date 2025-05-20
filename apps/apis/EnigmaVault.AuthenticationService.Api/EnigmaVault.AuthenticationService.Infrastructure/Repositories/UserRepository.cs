using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Domain.DomainModels;
using EnigmaVault.AuthenticationService.Infrastructure.Data;
using EnigmaVault.AuthenticationService.Infrastructure.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace EnigmaVault.AuthenticationService.Infrastructure.Repositories
{
    public class UserRepository(UsersDBContext context) : IUserRepository
    {
        private readonly UsersDBContext _context = context;

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

            await _context.Users.AddAsync(entity);
            await _context.SaveChangesAsync();
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Login.ToLower() == userDomain.Login.ToLower() && x.UserName.ToLower() == userDomain.UserName.ToLower());

            var domain = UserDomain.Reconstitute(
                user!.IdUser, user.Login, user.UserName, user.PasswordHash, user.Email, user.Phone,
                user.DateRegistration, user.DateEntry, user.DateUpdate,
                user.IdStatusAccount, user.IdGender, user.IdCountry, user.IdRole);

            return domain;
        }

        /*--Get-------------------------------------------------------------------------------------------*/

        public async Task<string> GetHashByLoginAsync(string login)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Login.ToLower() == login.ToLower());

            return user is null ? throw new ArgumentNullException("Пользователь не найден.") : user.PasswordHash;
        }

        public async Task<UserDomain?> GetUserByLogin(string login)
        {
            var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Login.ToLower() == login.ToLower());

            if (user is null) return null;

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
using EnigmaVault.AuthenticationService.Application.Abstractions.Repositories;
using EnigmaVault.AuthenticationService.Domain.DomainModels;
using EnigmaVault.AuthenticationService.Domain.ValueObjects;
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
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Login.ToLower() == userDomain.Login.ToLower() && x.UserName.ToLower() == userDomain.UserName.ToLower());

            Login.TryCreate(user.Login, out var loginResultVo);
            EmailAddress.TryCreate(user.Email, out var emailAddressVo);
            PhoneNumber.TryCreate(user.Phone, out var phoneNumberVo);

            var domain = UserDomain.Create(user.IdUser, loginResultVo, user.UserName, user.PasswordHash, emailAddressVo, phoneNumberVo, user.IdStatusAccount, user.IdGender, user.IdCountry, user.IdRole);

            return domain.User;
        }

        /*--Exist-----------------------------------------------------------------------------------------*/

        public async Task<bool> ExistsByLoginAsync(string login) => await _context.Users.AnyAsync(x => x.Login.ToLower() == login.ToLower());

        public async Task<bool> ExistsByEmailAsync(string email) => await _context.Users.AnyAsync(x => x.Email.ToLower() == email.ToLower());

        public async Task<bool> ExistsByPhoneAsync(string? phone)
        {
            if (phone is null) return false;
            return await _context.Users.AnyAsync(x => x.Phone == phone);
        }
    }
}
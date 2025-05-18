using EnigmaVault.AuthenticationService.Domain.Constants;
using EnigmaVault.AuthenticationService.Domain.ValueObjects;

namespace EnigmaVault.AuthenticationService.Domain.DomainModels
{
    public class UserDomain
    {
        private UserDomain() { }

        public int IdUser { get; private set; }
        public string Login { get; private set; } = null!;
        public string UserName { get; private set; } = null!;
        public string PasswordHash { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string? Phone { get; private set; }

        public DateTime DateRegistration { get; private set; }
        public DateTime DateEntry { get; private set; }
        public DateTime DateUpdate { get; private set; }

        public int? IdStatusAccount { get; private set; }
        public int? IdGender { get; private set; }
        public int? IdCountry { get; private set; }
        public int? IdRole { get; private set; }

        public static (UserDomain? User, List<string>? Errors) Create(
            int idUser, Login? login, string? userName,
            string passwordHash,
            EmailAddress? email, PhoneNumber? phone,
            int? idStatusAccount, int? idGender, int? idCountry, int? idRole)
        {
            List<string>? errors = null;

            if (email is null)
            {          
                errors ??= [];
                errors.Add($"Адрес электронной почты из ValueObject - EmailAddress был null.");
            }
            
            if (login is null)
            {
                errors ??= [];
                errors.Add($"Логин из ValueObject - Login был null.");
            }

            if (login is not null && string.IsNullOrWhiteSpace(login.Value))
            {
                errors ??= [];
                errors.Add($"Значение Value ValueObject - Login быо пустым.");
            }

            if (string.IsNullOrWhiteSpace(passwordHash))
            {
                errors ??= [];
                errors.Add($"Хеш пароля оказался пустым.");
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                errors ??= [];
                errors.Add($"Имя пользователя было пустым.");
            }

            if (!string.IsNullOrWhiteSpace(userName) && userName.Length > UserConstants.MaxUserNameLength)
            {
                errors ??= [];
                errors.Add($"Длинна имени пользователя превысило {UserConstants.MaxUserNameLength}");
            }
            
            if (!string.IsNullOrWhiteSpace(userName) && userName.Length < UserConstants.MinUserNameLength)
            {
                errors ??= [];
                errors.Add($"Длинна имени пользователя оказалось меньше чем {UserConstants.MinUserNameLength}");
            }

            if (errors is not null)
            {
                return (null, errors);
            }

            var dateRegistration = DateTime.UtcNow;
            var dateUpdate = DateTime.UtcNow;
            var dateEntry = DateTime.UtcNow;

            var user = new UserDomain()
            {
                IdUser = idUser,
                Login = login!.Value,
                UserName = userName!,
                PasswordHash = passwordHash,
                Email = email!.Value,
                Phone = phone?.Value,
                DateRegistration = dateRegistration,
                DateUpdate = dateUpdate,
                DateEntry = dateEntry,
                IdStatusAccount = idStatusAccount,
                IdGender = idGender,
                IdCountry = idCountry,
                IdRole = idRole
            };

            return (user, errors);
        }
    }
}
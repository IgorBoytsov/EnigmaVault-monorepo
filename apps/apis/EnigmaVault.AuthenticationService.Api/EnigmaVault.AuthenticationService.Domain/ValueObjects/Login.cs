using EnigmaVault.AuthenticationService.Domain.DomainModels.Validations;
using System.Text.RegularExpressions;

namespace EnigmaVault.AuthenticationService.Domain.ValueObjects
{
    public class Login : IEquatable<Login>
    {
        public string Value { get; }

        private Login(string value)
        {
            Value = value;
        }

        public static ValidationResults TryCreate(string? login, out Login? userLogin)
        {
            userLogin = null;
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(login))
            {
                errors.Add("Логин не может быть пустым.");
                return ValidationResults.Failure(errors);
            }
            if (!Regex.IsMatch(login, @"^[a-zA-Z]+$"))
            {
                errors.Add("Неверный формат логина. Допускаться только английские буквы любого регистра.");
            }

            if (errors.Any())
            {
                return ValidationResults.Failure(errors);
            }

            userLogin = new Login(login.ToLowerInvariant());

            return ValidationResults.Success();
        }

        public bool Equals(Login? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Login)obj);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(Login? left, Login? right) => Equals(left, right);
        public static bool operator !=(Login? left, Login? right) => !Equals(left, right);

        public override string ToString() => Value;
    }
}
using EnigmaVault.AuthenticationService.Domain.DomainModels.Validations;

namespace EnigmaVault.AuthenticationService.Domain.ValueObjects
{
    public class Password : IEquatable<Password>
    {
        public string Value { get; }

        private Password(string value)
        {
            Value = value;
        }

        public static ValidationResults TryCreate(string? password, out Password? userPassword)
        {
            userPassword = null;
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add("Пароль не может быть пустым.");
                return ValidationResults.Failure(errors);
            }

            if (!password!.Any(char.IsUpper))
                errors.Add("В пароле должна быть как минимум 1 буква верхнего регистра.");

            if (!password!.Any(char.IsLower))
                errors.Add("В пароле должна быть как минимум 1 буква нижнего регистра.");

            if (!password.Any(char.IsDigit))
                errors.Add("В пароле должна быть как минимум 1 цифра.");

            if (!password.Any(c => char.IsPunctuation(c) || char.IsSymbol(c)))
                errors.Add("В пароле должен быть как минимум 1 символ.");

            if (errors.Any())
                return ValidationResults.Failure(errors);

            userPassword = new Password(password!);

            return ValidationResults.Success();
        }

        public bool Equals(Password? other)
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

        public static bool operator ==(Password? left, Password? right) => Equals(left, right);
        public static bool operator !=(Password? left, Password? right) => !Equals(left, right);

        public override string ToString() => Value;
    }
}
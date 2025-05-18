using EnigmaVault.AuthenticationService.Domain.DomainModels.Validations;
using System.Text.RegularExpressions;

namespace EnigmaVault.AuthenticationService.Domain.ValueObjects
{
    public class EmailAddress : IEquatable<EmailAddress>
    {
        public string Value { get; }

        private EmailAddress(string value)
        {
            Value = value;
        }

        public static ValidationResults TryCreate(string? email, out EmailAddress? emailAddress)
        {
            emailAddress = null;
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add("Адрес электронной почты не может быть пустым.");
                return ValidationResults.Failure(errors);
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase))
            {
                errors.Add("Неверный формат адреса электронной почты.");
            }

            if (errors.Any())
            {
                return ValidationResults.Failure(errors);
            }

            emailAddress = new EmailAddress(email.ToLowerInvariant());

            return ValidationResults.Success();
        }

        public bool Equals(EmailAddress? other)
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
            return Equals((EmailAddress)obj);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(EmailAddress? left, EmailAddress? right) => Equals(left, right);
        public static bool operator !=(EmailAddress? left, EmailAddress? right) => !Equals(left, right);

        public override string ToString() => Value;
    }
}

using EnigmaVault.AuthenticationService.Domain.DomainModels.Validations;
using System.Text.RegularExpressions;

namespace EnigmaVault.AuthenticationService.Domain.ValueObjects
{
    public class PhoneNumber : IEquatable<PhoneNumber>
    {
        public string Value { get; } // Только цифры с +

        private PhoneNumber(string value)
        {
            Value = value;
        }

        public static ValidationResults TryCreate(string? phone, out PhoneNumber? phoneNumber)
        {
            phoneNumber = null;
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(phone)) 
                return ValidationResults.Success();

            string normalizedPhone = Regex.Replace(phone, @"[^\d+]", "");

            if (phone.Trim().StartsWith("+"))
                normalizedPhone = "+" + Regex.Replace(phone, @"[^\d]", "");
            else
                normalizedPhone = Regex.Replace(phone, @"[^\d]", "");

            if (!Regex.IsMatch(normalizedPhone, @"^\+?\d{7,15}$"))
                errors.Add("Неверный формат номера телефона. Ожидается от 7 до 15 цифр, возможно с '+' в начале.");
         
            const int minLength = 7;
            const int maxLength = 16; 
            if (normalizedPhone.Length < minLength && normalizedPhone.Length > 0)
                errors.Add($"Длина номера телефона должна быть не менее {minLength} цифр.");

            if (normalizedPhone.Length > maxLength)
                errors.Add($"Длина номера телефона не должна превышать {maxLength} символов.");

            if (errors.Any())
                return ValidationResults.Failure(errors);

            phoneNumber = new PhoneNumber(normalizedPhone);
            return ValidationResults.Success();
        }

        public static ValidationResults TryCreateOptional(string? phone, out PhoneNumber? phoneNumber)
        {
            phoneNumber = null;
            if (string.IsNullOrWhiteSpace(phone))
                return ValidationResults.Success();

            return TryCreate(phone, out phoneNumber);
        }

        public bool Equals(PhoneNumber? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((PhoneNumber)obj);
        }

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(PhoneNumber? left, PhoneNumber? right) => Equals(left, right);
        public static bool operator !=(PhoneNumber? left, PhoneNumber? right) => !Equals(left, right);

        public override string ToString() => Value;
    }
}
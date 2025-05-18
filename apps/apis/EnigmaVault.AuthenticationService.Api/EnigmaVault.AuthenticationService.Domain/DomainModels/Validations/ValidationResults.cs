namespace EnigmaVault.AuthenticationService.Domain.DomainModels.Validations
{
    public class ValidationResults
    {
        public bool IsValid { get; }

        public IReadOnlyList<string> Errors { get; }

        public string ErrorString => string.Join("; ", Errors);

        private ValidationResults(bool isValid, IReadOnlyList<string>? errors)
        {
            IsValid = isValid;
            Errors = errors ?? new List<string>();
        }

        public static ValidationResults Success() => new ValidationResults(true, null);

        public static ValidationResults Failure(string error) => new ValidationResults(false, [error]);

        public static ValidationResults Failure(List<string> errors) => new ValidationResults(false, errors);
    }
}
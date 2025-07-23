using EnigmaVault.SecretService.Domain.Results;

namespace EnigmaVault.SecretService.Application.Exceptions
{
    public class ValidationException(IReadOnlyCollection<Error> errors) : Exception("Произошла одна или несколько ошибок валидации.")
    {
        public IReadOnlyCollection<Error> Errors { get; } = errors;
    }
}
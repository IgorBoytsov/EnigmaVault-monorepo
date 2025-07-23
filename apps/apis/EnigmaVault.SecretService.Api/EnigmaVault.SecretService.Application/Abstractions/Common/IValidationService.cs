using FluentValidation.Results;

namespace EnigmaVault.SecretService.Application.Abstractions.Common
{
    public interface IValidationService
    {
        Task<ValidationResult> ValidateAsync<T>(T model);
    }
}
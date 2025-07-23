using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using FluentValidation.Results;

namespace EnigmaVault.SecretService.Application.Helpers
{
    public static class ValidationGuard
    {
        public static bool TryGetFailureResult<TResult>(ValidationResult validationResult, out Result<TResult> failureResult)
        {
            if (validationResult.IsValid)
            {
                failureResult = null!;
                return false;
            }

            var validationErrors = validationResult.Errors.Select(e => new Error(ErrorCode.Validation, $"{e.PropertyName}: {e.ErrorMessage}"));
            failureResult = Result<TResult>.Failure(validationErrors);
            return true;
        }

        public static bool TryGetFailureResult(ValidationResult validationResult, out Result failureResult)
        {
            if (validationResult.IsValid)
            {
                failureResult = null!;
                return false;
            }

            var validationErrors = validationResult.Errors.Select(e => new Error(ErrorCode.Validation, $"{e.PropertyName}: {e.ErrorMessage}")).ToList();
            failureResult = Result.Failure(validationErrors);
            return true;
        }
    }
}
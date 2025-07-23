using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;

namespace EnigmaVault.SecretService.Application.Helpers
{
    public static class RequestGuard
    {
        public static bool TryGetFailureResult<TRequest>(TRequest? request, out Result failureResult)
        {
            if (request is not null)
            {
                failureResult = null!;
                return false;
            }

            failureResult = Result.Failure(new Error(ErrorCode.NullValue, $"Запрос типа '{typeof(TRequest).Name}' не может быть null."));
            return true;
        }

        public static bool TryGetFailureResult<TRequest, TResult>(TRequest? request, out Result<TResult> failureResult)
        {
            if (request is not null)
            {
                failureResult = null!;
                return false;
            }

            failureResult = Result<TResult>.Failure(new Error(ErrorCode.NullValue, $"Запрос типа '{typeof(TRequest).Name}' не может быть null."));
            return true;
        }
    }
}
using EnigmaVault.AuthenticationService.Application.DTOs.CryptoParameters;
using EnigmaVault.AuthenticationService.Application.Enums;

namespace EnigmaVault.AuthenticationService.Application.DTOs.Results
{
    public class UserResult
    {
        public bool Success { get; private set; }
        public UserDto? User { get; private set; }
        public CryptoParameter? CryptoParameter { get; private set; }
        public ErrorCode? ErrorCode { get; private set; }
        public string? ErrorMessage { get; private set; }
        public List<string> ValidationErrors { get; private set; } = new List<string>();

        public static UserResult SuccessResult(UserDto user)
            => new()
            {
                Success = true,
                User = user
            }; 
        
        public static UserResult SuccessResult(UserDto user, CryptoParameter cryptoParameter)
            => new()
            {
                Success = true,
                User = user,
                CryptoParameter = cryptoParameter
            };
        
        public static UserResult SuccessResult() => new() { Success = true };

        public static UserResult FailureResult(ErrorCode errorCode, string? message = null)
            => new()
            {
                Success = false,
                ErrorCode = errorCode,
                ErrorMessage = message,
            };

        public static UserResult ValidationFailureResult(List<string> errors, string? generalMessage = null)
            => new()
            {
                Success = false,
                ErrorCode = Enums.ErrorCode.ValidationFailed,
                ValidationErrors = errors,
                ErrorMessage = generalMessage,
            };
    }
}
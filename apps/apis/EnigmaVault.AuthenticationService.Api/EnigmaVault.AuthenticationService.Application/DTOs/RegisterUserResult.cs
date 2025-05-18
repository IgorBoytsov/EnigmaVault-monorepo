using EnigmaVault.AuthenticationService.Application.Enums;

namespace EnigmaVault.AuthenticationService.Application.DTOs
{
    public class RegisterUserResult
    {
        public bool Success { get; private set; }
        public UserDto? User { get; private set; }
        public ErrorCode? ErrorCode { get; private set; } 
        public string? ErrorMessage { get; private set; } 
        public List<string> ValidationErrors { get; private set; } = new List<string>();

        public static RegisterUserResult SuccessResult(UserDto user)
            => new() 
            { 
                Success = true,
                User = user
            };

        public static RegisterUserResult FailureResult(ErrorCode errorCode, string? message = null)
            => new()
            { 
                Success = false, 
                ErrorCode = errorCode, 
                ErrorMessage = message,
            };

        public static RegisterUserResult ValidationFailureResult(List<string> errors, string? generalMessage = null)
            => new() 
            { 
                Success = false, 
                ErrorCode = Enums.ErrorCode.ValidationFailed, 
                ValidationErrors = errors, 
                ErrorMessage = generalMessage,
            };
    }
}
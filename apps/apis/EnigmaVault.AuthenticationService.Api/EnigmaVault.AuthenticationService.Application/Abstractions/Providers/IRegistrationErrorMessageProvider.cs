using EnigmaVault.AuthenticationService.Application.Enums;

namespace EnigmaVault.AuthenticationService.Application.Abstractions.Providers
{
    public interface IRegistrationErrorMessageProvider
    {
        string GetMessage(ErrorCode errorCode);
    }
}
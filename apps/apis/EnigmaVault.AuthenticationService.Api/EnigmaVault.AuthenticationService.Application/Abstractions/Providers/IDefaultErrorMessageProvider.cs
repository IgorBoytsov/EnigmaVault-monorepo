using EnigmaVault.AuthenticationService.Application.Enums;

namespace EnigmaVault.AuthenticationService.Application.Abstractions.Providers
{
    public interface IDefaultErrorMessageProvider
    {
        string GetMessage(ErrorCode errorCode);
    }
}
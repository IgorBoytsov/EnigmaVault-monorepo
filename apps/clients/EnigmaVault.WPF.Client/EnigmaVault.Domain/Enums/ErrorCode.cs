namespace EnigmaVault.Domain.Enums
{
    public enum ErrorCode
    {
        None,
        Unknown,

        AuthApiError,
        RegisterApiError,
        RecoveryApiError,
        NetworkError,

        InvalidResponseFormat,

        EmptyValue,
    }
}
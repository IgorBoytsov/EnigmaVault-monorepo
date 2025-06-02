namespace EnigmaVault.Domain.Enums
{
    public enum ErrorCode
    {
        None,
        Unknown,

        ApiError,

        AuthApiError,
        RegisterApiError,
        RecoveryApiError,
        NetworkError,

        InvalidResponseFormat,

        EmptyValue,
    }
}
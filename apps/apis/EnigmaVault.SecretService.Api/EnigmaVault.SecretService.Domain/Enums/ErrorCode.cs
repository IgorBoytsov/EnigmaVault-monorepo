namespace EnigmaVault.SecretService.Domain.Enums
{
    public enum ErrorCode
    {
        None,
        NullValue,
        Empty,
        NotFound,

        SaveError,
        DeleteError,
    }
}
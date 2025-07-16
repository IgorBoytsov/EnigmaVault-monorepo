namespace EnigmaVault.SecretService.Domain.Enums
{
    public enum ErrorCode
    {
        None,
        NullValue,
        Empty,
        NotFound,

        CreateError,
        SaveError,
        UpdateError,
        DeleteError,
        CannotBeUpdated,
    }
}
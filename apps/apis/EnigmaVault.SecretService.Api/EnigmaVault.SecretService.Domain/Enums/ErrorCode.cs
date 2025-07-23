namespace EnigmaVault.SecretService.Domain.Enums
{
    public enum ErrorCode
    {
        None,
        UnknownError,
        NullValue,
        Empty,
        Validation,

        NotFound,

        ConcurrencyError,

        CreateError,
        SaveError,
        UpdateError,
        DeleteError,
        CannotBeUpdated,

        DomainError,
    }
}
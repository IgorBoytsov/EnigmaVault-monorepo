using EnigmaVault.SecretService.Domain.Enums;

namespace EnigmaVault.SecretService.Domain.Results
{
    public record Error(ErrorCode? Code, string Description)
    {
        public static readonly Error None = new(ErrorCode.None, string.Empty);
    }
}
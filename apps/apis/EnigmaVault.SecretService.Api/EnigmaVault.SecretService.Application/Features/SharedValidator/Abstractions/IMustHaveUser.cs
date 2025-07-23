namespace EnigmaVault.SecretService.Application.Features.SharedValidator.Abstractions
{
    public interface IMustHaveUser
    {
        int UserId { get; }
    }
}
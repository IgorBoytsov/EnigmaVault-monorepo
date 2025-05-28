using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.UserCase
{
    public interface IRegistrationUserUseCase
    {
        Task<Result<(string Login, string Password)?>> RegistrationAsync(string login, string password, string userName, string email, string? phone, int idGender, int idCountry);
    }
}
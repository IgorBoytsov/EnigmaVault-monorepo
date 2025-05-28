using EnigmaVault.Application.Dtos;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.CountryCase
{
    public interface IGetCountriesUseCase
    {
        Task<Result<List<CountryDto>?>> GetCountriesAsync();
    }
}
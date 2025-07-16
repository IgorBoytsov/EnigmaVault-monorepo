using EnigmaVault.Application.Dtos;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.Abstractions.Repositories
{
    public interface ICountryRepository
    {
        Task<Result<List<CountryDto>>> GetGendersAsync();
    }
}
using EnigmaVault.Domain.DomainModels;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.Abstractions.Repositories
{
    public interface ICountryRepository
    {
        Task<Result<List<CountryDomain>?>> GetGendersAsync();
    }
}
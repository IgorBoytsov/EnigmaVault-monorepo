using EnigmaVault.Domain.DomainModels;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.Abstractions.Repositories
{
    public interface IGenderRepository
    {
        Task<Result<List<GenderDomain>?>> GetGendersAsync();
    }
}
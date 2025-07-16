using EnigmaVault.Application.Dtos;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.Abstractions.Repositories
{
    public interface IGenderRepository
    {
        Task<Result<List<GenderDto>>> GetGendersAsync();
    }
}
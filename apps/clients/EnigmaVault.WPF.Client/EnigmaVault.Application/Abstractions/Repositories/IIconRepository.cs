using EnigmaVault.Application.Dtos;
using EnigmaVault.Domain.DomainModels;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.Abstractions.Repositories
{
    public interface IIconRepository
    {
        Task<Result<IconDomain?>> CreateAsync(IconDomain icon);
        Task<Result<List<IconDto>?>> GetAllAsync(int userId);
        Task<Result> UpdateAsync(int idUser, int idIcon, string name, string? svgCode);
    }
}
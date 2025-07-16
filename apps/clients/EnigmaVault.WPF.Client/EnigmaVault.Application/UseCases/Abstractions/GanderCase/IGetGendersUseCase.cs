using EnigmaVault.Application.Dtos;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.GanderCase
{
    public interface IGetGendersUseCase
    {
        Task<Result<List<GenderDto>>> GetGendersAsync();
    }
}
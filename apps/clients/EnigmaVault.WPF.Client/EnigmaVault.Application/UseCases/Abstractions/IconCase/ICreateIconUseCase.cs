using EnigmaVault.Application.Dtos;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.IconCase
{
    public interface ICreateIconUseCase
    {
        Task<Result<IconDto>> CreateAsync(int? idUser, string svgCode, string iconName);
    }
}
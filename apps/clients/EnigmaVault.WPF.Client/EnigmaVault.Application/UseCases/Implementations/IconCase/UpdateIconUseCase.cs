using EnigmaVault.Application.Abstractions.Repositories;
using EnigmaVault.Application.UseCases.Abstractions.IconCase;
using EnigmaVault.Domain.Enums;
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Implementations.IconCase
{
    public class UpdateIconUseCase(IIconRepository iconRepository) : IUpdateIconUseCase
    {
        private readonly IIconRepository _iconRepository = iconRepository;

        public async Task<Result> UpdateNameAsync(int idUser, int idIcon, string name, string? svgCode)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure(new Error(ErrorCode.EmptyValue, "Название иконки не может быть пустым"));

            if (string.IsNullOrWhiteSpace(svgCode))
                return Result.Failure(new Error(ErrorCode.EmptyValue, "Код иконки не может быть пустым"));

            return await _iconRepository.UpdateAsync(idUser, idIcon, name, svgCode);
        }
    }
}
using EnigmaVault.Domain.Results;

namespace EnigmaVault.Application.UseCases.Abstractions.SecretCase
{
    public interface IUpdateNoteUseCase
    {
        Task<Result<DateTime>> UpdateNoteAsync(int idSecret, string? note);
    }
}
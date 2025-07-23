using EnigmaVault.SecretService.Application.Features.SharedValidator.Implementations;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Folders.GetAll
{
    public sealed class GetAllFoldersQueryValidator : AbstractValidator<GetAllFoldersQuery>
    {
        public GetAllFoldersQueryValidator()
        {
            Include(new MustHaveUserValidator());
        }
    }
}
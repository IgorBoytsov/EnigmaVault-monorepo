using EnigmaVault.SecretService.Application.Features.SharedValidator.Implementations;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Icons.GetAll
{
    public sealed class GetAllIconsQueryValidator : AbstractValidator<GetAllIconsQuery>
    {
        public GetAllIconsQueryValidator()
        {
            Include(new MayHaveUserValidator());
        }
    }
}
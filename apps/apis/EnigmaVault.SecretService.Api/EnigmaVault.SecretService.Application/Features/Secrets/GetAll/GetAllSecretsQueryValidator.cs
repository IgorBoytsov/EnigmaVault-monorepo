using EnigmaVault.SecretService.Application.Features.SharedValidator.Implementations;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Secrets.GetAll
{
    public sealed class GetAllSecretsQueryValidator : AbstractValidator<GetAllSecretsQuery>
    {
        public GetAllSecretsQueryValidator()
        {
            Include(new MustHaveUserValidator());
        }
    }
}
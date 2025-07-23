using EnigmaVault.SecretService.Application.Features.Secrets.Validators.Implementations;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Secrets.GetByIdSecret
{
    public sealed class GetSecretByIdQueryValidator : AbstractValidator<GetSecretByIdQuery>
    {
        public GetSecretByIdQueryValidator()
        {
            Include(new IdSecretDataValidator());
        }
    }
}
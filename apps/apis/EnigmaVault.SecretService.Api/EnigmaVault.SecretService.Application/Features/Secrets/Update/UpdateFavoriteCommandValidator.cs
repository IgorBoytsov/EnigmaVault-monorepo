using EnigmaVault.SecretService.Application.Features.Secrets.Validators.Implementations;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed class UpdateFavoriteCommandValidator : AbstractValidator<UpdateFavoriteCommand>
    {
        public UpdateFavoriteCommandValidator()
        {
            Include(new IdSecretDataValidator());
        }
    }
}
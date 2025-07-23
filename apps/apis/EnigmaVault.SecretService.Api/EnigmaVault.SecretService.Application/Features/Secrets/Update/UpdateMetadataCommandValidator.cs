using EnigmaVault.SecretService.Application.Features.Secrets.Validators.Implementations;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed class UpdateMetadataCommandValidator : AbstractValidator<UpdateMetadataCommand>
    {
        public UpdateMetadataCommandValidator()
        {
            Include(new IdSecretDataValidator());
            Include(new ServiceNameDataValidator());
        }
    }
}

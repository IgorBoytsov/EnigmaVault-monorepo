using EnigmaVault.SecretService.Application.Features.Secrets.Validators.Abstractions;
using EnigmaVault.SecretService.Domain.Constants;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Validators.Implementations
{
    public sealed class ServiceNameDataValidator : AbstractValidator<IServiceNameDataHolder>
    {
        public ServiceNameDataValidator()
        {
            RuleFor(x => x.ServiceName)
                .NotEmpty().WithMessage("Название папки не может быть пустым.")
                .Length(SecretConstants.MIN_SERVICENAME_LENGTH, SecretConstants.MAX_SERVICENAME_LENGTH).WithMessage($"Минимально допустимая длинна - {SecretConstants.MIN_SERVICENAME_LENGTH}, а максимально допустимая длина - {SecretConstants.MAX_SERVICENAME_LENGTH}");
        }
    }
}
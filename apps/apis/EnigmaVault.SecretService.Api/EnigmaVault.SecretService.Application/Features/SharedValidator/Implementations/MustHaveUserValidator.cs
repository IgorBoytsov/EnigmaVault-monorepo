using EnigmaVault.SecretService.Application.Features.SharedValidator.Abstractions;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.SharedValidator.Implementations
{
    public sealed class MustHaveUserValidator : AbstractValidator<IMustHaveUser>
    {
        public MustHaveUserValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("ID пользователя не может быть меньше 0");
        }
    }
}
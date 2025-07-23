using EnigmaVault.SecretService.Application.Features.SharedValidator.Abstractions;
using FluentValidation;

namespace EnigmaVault.SecretService.Application.Features.SharedValidator.Implementations
{
    public sealed class MayHaveUserValidator : AbstractValidator<IMayHaveUser>
    {
        public MayHaveUserValidator()
        {
            When(x => x.IdUser.HasValue, () =>
            {
                RuleFor(x => x.IdUser)
                    .GreaterThan(0).WithMessage("ID пользователя не может быть меньше 0");
            });
        }
    }
}
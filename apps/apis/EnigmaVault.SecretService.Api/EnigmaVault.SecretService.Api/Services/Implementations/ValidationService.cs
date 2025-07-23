using EnigmaVault.SecretService.Application.Abstractions.Common;
using FluentValidation;
using FluentValidation.Results;

namespace EnigmaVault.SecretService.Api.Services.Implementations
{
    public class ValidationService : IValidationService
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<ValidationResult> ValidateAsync<T>(T model)
        {
            if (model == null) 
                return new ValidationResult([new ValidationFailure("", "Модель не может быть null")]);

            var validator = _serviceProvider.GetService<IValidator<T>>();

            if (validator == null)
                return new ValidationResult();

            return await validator.ValidateAsync(model);
        }
    }
}
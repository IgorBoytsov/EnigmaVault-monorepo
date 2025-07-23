using EnigmaVault.SecretService.Application.Abstractions.Common;
using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Helpers;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Exceptions;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Icons.Update
{
    public sealed class UpdateIconCommandHandler(IIconRepository iconRepository, IValidationService validator) : IRequestHandler<UpdateIconCommand, Result>
    {
        private readonly IIconRepository _iconRepository = iconRepository;
        private readonly IValidationService _validator = validator;

        public async Task<Result> Handle(UpdateIconCommand request, CancellationToken cancellationToken)
        {
            if (RequestGuard.TryGetFailureResult<UpdateIconCommand>(request, out var nullFailureResult))
                return nullFailureResult;

            var validationResult = await _validator.ValidateAsync(request);

            if (ValidationGuard.TryGetFailureResult(validationResult, out var validationFailureResult))
                return validationFailureResult;

            var iconDomain = await _iconRepository.GetByIdAsync(request.UserId, request.IdIcon);

            if (iconDomain is null)
                return Result.Failure(new Error(ErrorCode.NotFound, "Не удалось найти иконку."));

            if (!iconDomain.CanBeUpdated())
                return Result.Failure(new Error(ErrorCode.CannotBeUpdated, "Иконку нельзя обновить т.к она является общей."));

            try
            {
                iconDomain.UpdateName(request.IconName);
                iconDomain.UpdateSvgCode(request.SvgCode);
            }
            catch (DomainException ex)
            {
                return Result.Failure(new Error(ErrorCode.UpdateError, ex.Message));
            }
            catch (Exception)
            {
                return Result.Failure(new Error(ErrorCode.UpdateError, "Произошла непредвиденная ошибка при обновление значений"));
            }

            var result = await _iconRepository.UpdateIconAsync(iconDomain);

            return result;
        }
    }
}
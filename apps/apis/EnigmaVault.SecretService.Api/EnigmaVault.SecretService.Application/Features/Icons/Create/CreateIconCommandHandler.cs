using EnigmaVault.SecretService.Application.Abstractions.Common;
using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Helpers;
using EnigmaVault.SecretService.Application.Mappers;
using EnigmaVault.SecretService.Domain.DomainModels;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Exceptions;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Icons.Create
{
    public sealed class CreateIconCommandHandler(IIconRepository iconRepository, IValidationService validator) : IRequestHandler<CreateIconCommand, Result<IconDto>>
    {
        private readonly IIconRepository _iconRepository = iconRepository;
        private readonly IValidationService _validator = validator;

        public async Task<Result<IconDto>> Handle(CreateIconCommand request, CancellationToken cancellationToken)
        {
            if (RequestGuard.TryGetFailureResult<CreateIconCommand, IconDto>(request, out var nullFailureResult))
                return nullFailureResult;

            var validationResult = await _validator.ValidateAsync(request);

            if (ValidationGuard.TryGetFailureResult<IconDto>(validationResult, out var validationFailureResult))
                return validationFailureResult;

            try
            {
                var domain = IconDomain.Create(request.IdUser, request.SvgCode, request.IconName, request.IsCommon);

                var result = await _iconRepository.CreateAsync(domain);

                if (!result.IsSuccess)
                    return Result<IconDto>.Failure(result.Errors);

                return Result<IconDto>.Success(result.Value!.ToDto());
            }
            catch (DomainException ex)
            {
                return Result<IconDto>.Failure(new Error(ErrorCode.CreateError, ex.Message));
            }
        }
    }
}
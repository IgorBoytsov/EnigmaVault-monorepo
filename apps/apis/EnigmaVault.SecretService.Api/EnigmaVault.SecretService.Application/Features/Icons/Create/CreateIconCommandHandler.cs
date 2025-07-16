using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Application.Mappers;
using EnigmaVault.SecretService.Domain.DomainModels;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Exceptions;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Icons.Create
{
    public class CreateIconCommandHandler(IIconRepository iconRepository) : IRequestHandler<CreateIconCommand, Result<IconDto>>
    {
        private readonly IIconRepository _iconRepository = iconRepository;

        public async Task<Result<IconDto>> Handle(CreateIconCommand request, CancellationToken cancellationToken)
        {
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
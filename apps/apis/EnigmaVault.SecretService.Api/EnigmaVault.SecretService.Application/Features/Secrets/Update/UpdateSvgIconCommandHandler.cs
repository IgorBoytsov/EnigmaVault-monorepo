using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public class UpdateSvgIconCommandHandler(ISecretRepository secretRepository) : IRequestHandler<UpdateSvgIconCommand, Result<DateTime>>
    {
        public readonly ISecretRepository _secretRepository = secretRepository;

        public async Task<Result<DateTime>> Handle(UpdateSvgIconCommand request, CancellationToken cancellationToken)
        {
            var storage = await _secretRepository.GetByIdAsync(request.IdSecret, cancellationToken);

            if (storage is null)
                return Result<DateTime>.Failure(new Error(ErrorCode.NotFound, "Данной записи не существует."));

            storage.UpdateSvgIcon(request.SvgIcon);

            var result = await _secretRepository.UpdateAsync(storage);

            return result;
        }
    }
}
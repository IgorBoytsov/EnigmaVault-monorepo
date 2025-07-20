using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public sealed class UpdateIsArchiveCommandHandler(ISecretRepository secretRepository) : IRequestHandler<UpdateIsArchiveCommand, Result>
    {
        private readonly ISecretRepository _secretRepository = secretRepository;

        public async Task<Result> Handle(UpdateIsArchiveCommand request, CancellationToken cancellationToken)
        {
            var storage = await _secretRepository.GetByIdAsync(request.IdSecret, cancellationToken);

            if (storage is null)
                return Result<DateTime>.Failure(new Error(ErrorCode.NotFound, "Данной записи не существует."));

            storage.ToggleArchive(request.IsArchive);

            var result = await _secretRepository.UpdateAsync(storage);

            if (!result.IsSuccess)
                return Result.Failure(result.Errors);

            return Result.Success();
        }
    }
}
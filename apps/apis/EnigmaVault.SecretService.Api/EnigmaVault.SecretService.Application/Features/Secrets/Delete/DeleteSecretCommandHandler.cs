using EnigmaVault.SecretService.Application.Abstractions;
using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Delete
{
    public class DeleteSecretCommandHandler(ISecretRepository secretRepository, IUnitOfWork unitOfWork) : IRequestHandler<DeleteSecretCommand, Result>
    {
        private readonly ISecretRepository _secretRepository = secretRepository;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<Result> Handle(DeleteSecretCommand request, CancellationToken cancellationToken)
        {
            var domain = await _secretRepository.GetByIdAsync(request.IdSecret, cancellationToken);

            if (domain == null)
                return Result.Failure(new Error(ErrorCode.NotFound, "Не удалось найти запись"));

            if (domain.IsArchive)
                return Result.Failure(new Error(ErrorCode.DeleteError, "Нельзя удалить запись которая находиться в архиве"));

            var isDelete = await _secretRepository.DeleteAsync(request.IdSecret);

            if (isDelete)
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return await Task.FromResult(Result.Success());
            }
            else
                return await Task.FromResult(Result.Failure(new Error(ErrorCode.DeleteError, "Не удалось удалить запись")));
        }
    }
}
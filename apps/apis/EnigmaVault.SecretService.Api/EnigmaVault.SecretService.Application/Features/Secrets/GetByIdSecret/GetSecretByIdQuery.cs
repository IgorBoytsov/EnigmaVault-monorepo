using EnigmaVault.SecretService.Application.Features.Secrets.Validators.Abstractions;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.GetByIdSecret
{
    public sealed record GetSecretByIdQuery(int IdSecret) : IRequest<Result<SecretDto>>, IIdSecretDataHolder;
}
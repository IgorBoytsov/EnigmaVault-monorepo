using EnigmaVault.SecretService.Application.Features.Secrets.Validators.Abstractions;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Delete
{
    public sealed record DeleteSecretCommand(int IdSecret) : IRequest<Result>, IIdSecretDataHolder;
}
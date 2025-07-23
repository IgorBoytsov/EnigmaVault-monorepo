using EnigmaVault.SecretService.Application.Features.SharedValidator.Abstractions;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.GetAll
{
    public sealed record GetAllSecretsQuery(int UserId) : IStreamRequest<SecretDto>, IMustHaveUser;
}
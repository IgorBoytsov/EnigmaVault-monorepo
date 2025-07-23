using EnigmaVault.SecretService.Application.Features.SharedValidator.Abstractions;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Icons.GetAll
{
    public record GetAllIconsQuery(int? IdUser) : IStreamRequest<IconDto>, IMayHaveUser;
}
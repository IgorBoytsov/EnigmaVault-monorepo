using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Icons.GetAll
{
    public record GetAllIconsQuery(int? IdUser) : IRequest<IAsyncEnumerable<IconDto>>
    {
    }
}
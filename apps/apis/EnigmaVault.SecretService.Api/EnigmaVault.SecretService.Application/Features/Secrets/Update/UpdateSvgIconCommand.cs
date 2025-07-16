using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Secrets.Update
{
    public record UpdateSvgIconCommand(int IdSecret, string? SvgIcon) : IRequest<Result<DateTime>>
    {
    }
}

using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Icons.Update
{
    public record UpdateIconCommand(int IdUser, int IdIcon, string IconName, string SvgCode) : IRequest<Result>
    {
    }
}
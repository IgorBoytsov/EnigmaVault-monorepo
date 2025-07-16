using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Icons.Create
{
    public record CreateIconCommand(int? IdUser, string SvgCode, string IconName, bool IsCommon) : IRequest<Result<IconDto>>
    {
    }
}
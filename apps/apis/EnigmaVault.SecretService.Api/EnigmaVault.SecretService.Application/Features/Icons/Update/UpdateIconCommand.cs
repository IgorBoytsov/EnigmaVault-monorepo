using EnigmaVault.SecretService.Application.Features.Icons.Validators.Abstractions;
using EnigmaVault.SecretService.Application.Features.SharedValidator.Abstractions;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Icons.Update
{
    public record UpdateIconCommand(
        int UserId,
        int IdIcon, 
        string IconName, 
        string SvgCode) : IRequest<Result>, IMustHaveUser, IIconNameValidator, ISvgCodeValidator;
}
using EnigmaVault.SecretService.Domain.Results;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Folders.Update
{
    public class UpdateFolderCommand : IRequest<Result>
    {
        public int IdFolder {  get; set; }
        public string Name { get; set; } = null!;
    }
}
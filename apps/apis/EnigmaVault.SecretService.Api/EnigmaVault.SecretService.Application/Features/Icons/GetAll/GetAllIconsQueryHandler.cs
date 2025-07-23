using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using MediatR;
using System.Runtime.CompilerServices;

namespace EnigmaVault.SecretService.Application.Features.Icons.GetAll
{
    public sealed class GetAllIconsQueryHandler(IIconRepository iconRepository) : IStreamRequestHandler<GetAllIconsQuery, IconDto>
    {
        private readonly IIconRepository _iconRepository = iconRepository;

        //TODO: Реализовать валидацию с исключениями. В API перехватывать их через Middleware
        public async IAsyncEnumerable<IconDto> Handle(GetAllIconsQuery request, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var iconsStream = _iconRepository.GetAllStreamingAsync(request.IdUser, cancellationToken);

            await foreach (var icon in iconsStream.WithCancellation(cancellationToken))
                yield return icon;
        }
    }
}
using EnigmaVault.SecretService.Application.Abstractions.Repositories;
using MediatR;

namespace EnigmaVault.SecretService.Application.Features.Icons.GetAll
{
    public class GetAllIconsQueryHandler(IIconRepository iconRepository) : IRequestHandler<GetAllIconsQuery, IAsyncEnumerable<IconDto>>
    {
        private readonly IIconRepository _iconRepository = iconRepository;

        public Task<IAsyncEnumerable<IconDto>> Handle(GetAllIconsQuery request, CancellationToken cancellationToken)
            => Task.FromResult(_iconRepository.GetAllStreamingAsync(request.IdUser, cancellationToken));
    }
}
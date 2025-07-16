using EnigmaVault.SecretService.Application.Features.IconCategories;
using EnigmaVault.SecretService.Application.Features.IconCategories.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EnigmaVault.SecretService.Api.Controllers
{
    [Route("api/icon-category")]
    [ApiController]
    public class IconCategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public IconCategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("get-all")]
        public Task<IAsyncEnumerable<IconCategoryDto>> GetAll(CancellationToken cancellationToken) => _mediator.Send(new GetAllIconCategoriesQuery(), cancellationToken);
    }
}
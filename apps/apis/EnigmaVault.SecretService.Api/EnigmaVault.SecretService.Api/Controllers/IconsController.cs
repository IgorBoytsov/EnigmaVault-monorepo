using EnigmaVault.SecretService.Api.Dtos.Requests.Icons;
using EnigmaVault.SecretService.Application.Features.Icons;
using EnigmaVault.SecretService.Application.Features.Icons.Create;
using EnigmaVault.SecretService.Application.Features.Icons.GetAll;
using EnigmaVault.SecretService.Application.Features.Icons.Update;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EnigmaVault.SecretService.Api.Controllers
{
    [Route("api/icons")]
    [ApiController]
    public class IconsController : Controller
    {
        private readonly IMediator _mediator;

        public IconsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /*--Create----------------------------------------------------------------------------------------*/

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateIconCommand command)
        {
            Result<IconDto> result = await _mediator.Send(command);

            if (result.IsSuccess)
                return StatusCode(StatusCodes.Status201Created, result.Value);
            else
                return BadRequest(result.Errors);
        }

        /*--Get-------------------------------------------------------------------------------------------*/
        
        [HttpGet("get-all")]
        public Task<IAsyncEnumerable<IconDto>> GetAll([FromQuery] GetAllIconsQuery query, CancellationToken cancellationToken) => _mediator.Send(query, cancellationToken);

        /*--Update----------------------------------------------------------------------------------------*/

        [HttpPatch("{id}/update")]
        public async Task<IActionResult> UpdateIcon([FromRoute] int id, [FromBody] UpdateIconRequest request)
        {
            var command = new UpdateIconCommand(request.IdUser, id, request.IconName, request.SvgCode);

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                var notFoundError = result.Errors.FirstOrDefault(r => r.Code == ErrorCode.NotFound);
                if (notFoundError is not null)
                    return NotFound(notFoundError.Description);

                return BadRequest(result.Errors);
            }
            
            return Ok();
        }

    }
}

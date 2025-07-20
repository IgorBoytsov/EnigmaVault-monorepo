using EnigmaVault.SecretService.Api.Dtos.Requests.Folders;
using EnigmaVault.SecretService.Application.Features.Folders;
using EnigmaVault.SecretService.Application.Features.Folders.Create;
using EnigmaVault.SecretService.Application.Features.Folders.Delete;
using EnigmaVault.SecretService.Application.Features.Folders.GetAll;
using EnigmaVault.SecretService.Application.Features.Folders.Update;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EnigmaVault.SecretService.Api.Controllers
{
    [Route("api/folders")]
    [ApiController]
    public sealed class FoldersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FoldersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /*--Create----------------------------------------------------------------------------------------*/

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSecret([FromBody] CreateFolderCommand command)
        {
            Result<FolderDto> result = await _mediator.Send(command);

            if (result.IsSuccess)
                return StatusCode(StatusCodes.Status201Created, result.Value);
            else
                return BadRequest(result.Errors);
        }

        /*--Get-------------------------------------------------------------------------------------------*/

        [HttpGet("get-all")]
        public Task<IAsyncEnumerable<FolderDto>> GetAll([FromQuery] GetAllFoldersQuery query, CancellationToken cancellationToken) => _mediator.Send(query, cancellationToken);

        /*--Update----------------------------------------------------------------------------------------*/
        
        [HttpPut("{id}/name")]
        public async Task<IActionResult> UpdateName([FromRoute] int id, [FromBody] UpdateFolderRequest request)
        {
            var command = new UpdateFolderCommand(id, request.Name);

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
            {
                var notFoundCode = result.Errors.FirstOrDefault(error => error.Code == ErrorCode.NotFound);
                if (notFoundCode != null)
                    return NotFound(result.Errors);

                return BadRequest(result.Errors);
            }

            return Ok();
        }

        /*--Delete----------------------------------------------------------------------------------------*/

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var command = new DeleteFolderCommand(id);

            var result = await _mediator.Send(command);
            if (!result.IsSuccess)
            {
                var error = result.Errors.FirstOrDefault();
                if (error!.Code == ErrorCode.NotFound)
                    return NotFound(result.Errors);
                if (error!.Code == ErrorCode.DeleteError)
                    return StatusCode(StatusCodes.Status500InternalServerError);

                return BadRequest(result.Errors);
            }

            return NoContent();
        }
    }
}
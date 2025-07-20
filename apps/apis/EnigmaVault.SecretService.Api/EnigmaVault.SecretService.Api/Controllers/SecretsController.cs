using EnigmaVault.SecretService.Api.Dtos.Requests.Secrets;
using EnigmaVault.SecretService.Api.Dtos.Responses;
using EnigmaVault.SecretService.Application.Features.Secrets;
using EnigmaVault.SecretService.Application.Features.Secrets.Create;
using EnigmaVault.SecretService.Application.Features.Secrets.Delete;
using EnigmaVault.SecretService.Application.Features.Secrets.GetAll;
using EnigmaVault.SecretService.Application.Features.Secrets.GetByIdSecret;
using EnigmaVault.SecretService.Application.Features.Secrets.Update;
using EnigmaVault.SecretService.Domain.Enums;
using EnigmaVault.SecretService.Domain.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EnigmaVault.SecretService.Api.Controllers
{
    [Route("api/secrets")]
    [ApiController]
    public sealed class SecretsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SecretsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /*--Create----------------------------------------------------------------------------------------*/

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSecret([FromBody] CreateSecretCommand command)
        {
            Result<SecretDto> result = await _mediator.Send(command);

            if (result.IsSuccess)
                return StatusCode(StatusCodes.Status201Created, result.Value);
            else
                return BadRequest(result.Errors);
        }

        /*--Get-------------------------------------------------------------------------------------------*/

        [HttpGet("get-all")]
        public Task<IAsyncEnumerable<SecretDto>> GetAll([FromQuery] GetAllSecretsQuery query, CancellationToken cancellationToken) => _mediator.Send(query, cancellationToken);

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SecretDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSecretById([FromRoute] int id)
        {
            var query = new GetSecretByIdQuery(id);

            Result<SecretDto> result = await _mediator.Send(query);

            if (!result.IsSuccess)
                return NotFound(result.Errors);

            return Ok(result.Value);
        }

        /*--Update----------------------------------------------------------------------------------------*/

        [HttpPut("{id}/all-secret")]
        [ProducesResponseType(typeof(UpdateSecretResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSecret([FromRoute] int id, [FromBody] UpdateSecretRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new UpdateSecretCommand(id, request.ServiceName, request.Url, request.IsFavorite, request.Note, request.EncryptedData, request.Nonce, request.SchemaVersion);

            Console.WriteLine(command.Note);
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                var response = new UpdateSecretResponse(result.Value);
                return Ok(response);
            }
            else
            {
                var firstError = result.Errors[0];
                if (firstError.Code == ErrorCode.NotFound)
                    return NotFound(result.Errors);

                return BadRequest(result.Errors);
            }
        }

        [HttpPatch("{id}/encrypted-data")]
        [ProducesResponseType(typeof(UpdateSecretResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEncryptedData([FromRoute] int id, [FromBody] UpdateEncryptedDataRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new UpdateEncryptedDataCommand(id, request.EncryptedData, request.Nonce, request.SchemaVersion);

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                var response = new UpdateSecretResponse(result.Value);

                return Ok(response);
            }
            else
            {
                var firstError = result.Errors[0];

                if (firstError.Code == ErrorCode.NotFound)
                    return NotFound(result.Errors);

                return BadRequest(result.Errors);
            }
        }

        [HttpPatch("{id}/meta-data")]
        [ProducesResponseType(typeof(UpdateSecretResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMetadata([FromRoute] int id, [FromBody] UpdateMetadataRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new UpdateMetadataCommand(id, request.ServiceName, request.Url);

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                var response = new UpdateSecretResponse(result.Value);

                return Ok(response);
            }
            else
            {
                var firstError = result.Errors[0];

                if (firstError.Code == ErrorCode.NotFound)
                    return NotFound(result.Errors);

                return BadRequest(result.Errors);
            }
        }

        [HttpPatch("{id}/favorite")]
        [ProducesResponseType(typeof(UpdateSecretResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateFavorite([FromRoute] int id, [FromBody] UpdateFavoriteRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new UpdateFavoriteCommand(id, request.IsFavorite);

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                var response = new UpdateSecretResponse(result.Value);

                return Ok(response);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        [HttpPatch("{id}/note")]
        [ProducesResponseType(typeof(UpdateSecretResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateNote([FromRoute] int id, [FromBody] UpdateNoteRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new UpdateNoteCommand(id, request.Note);

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                var response = new UpdateSecretResponse(result.Value);

                return Ok(response);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        [HttpPatch("{id}/icon")]
        [ProducesResponseType(typeof(UpdateSecretResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateSvgIcon([FromRoute] int id, [FromBody] UpdateSvgIconInSecretRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new UpdateSvgIconCommand(id, request.SvgIcon);

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                var response = new UpdateSecretResponse(result.Value);

                return Ok(response);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        [HttpPatch("{id}/folder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateFolder([FromRoute] int id, [FromBody] UpdateSecretFolderRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new UpdateSecretFolderCommand(id, request.IdFolder);

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return Ok();
            else
                return NotFound(result.Errors);
        }

        [HttpPatch("{id}/isArchive")]
        public async Task<IActionResult> UpdateIsArchive([FromRoute] int id, [FromBody] UpdateIsArchiveRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var command = new UpdateIsArchiveCommand(id, request.IsArchive);

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
                return Ok();
            else
                return NotFound(result.Errors);
        }

        /*--Delete----------------------------------------------------------------------------------------*/

        [HttpDelete("{id}")] 
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteSecret([FromRoute] int id)
        {
            var command = new DeleteSecretCommand(id);

            var result = await _mediator.Send(command);


            if (result.IsSuccess)
            {
                return NoContent();
            }

            var firstError = result.Errors.FirstOrDefault();

            if (firstError?.Code == ErrorCode.DeleteError)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return BadRequest(result.Errors);
        }

    }
}
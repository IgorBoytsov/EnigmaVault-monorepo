using EnigmaVault.AuthenticationService.Api.DTOs.Requests;
using EnigmaVault.AuthenticationService.Api.DTOs.Responses;
using EnigmaVault.AuthenticationService.Api.Mappers;
using EnigmaVault.AuthenticationService.Application.Abstractions.Providers;
using EnigmaVault.AuthenticationService.Application.Abstractions.UseCases;
using EnigmaVault.AuthenticationService.Application.DTOs;
using EnigmaVault.AuthenticationService.Application.Enums;
using Microsoft.AspNetCore.Mvc;

namespace EnigmaVault.AuthenticationService.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IRegisterUserUseCase _registerUserUseCase;
        private readonly IRegistrationErrorMessageProvider _registrationErrorMessageProvider;

        public UsersController(IRegisterUserUseCase registerUserUseCase,
                               IRegistrationErrorMessageProvider registrationErrorMessageProvider)
        {
            _registerUserUseCase = registerUserUseCase;
            _registrationErrorMessageProvider = registrationErrorMessageProvider;
        }

        /*--Регистрация-----------------------------------------------------------------------------------*/

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterUserApiRequest apiRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            RegisterUserCommand command = apiRequest.ToMapCommand();

            RegisterUserResult result = await _registerUserUseCase.RegisterAsync(command);

            if (!result.Success)
            {
                Func<IActionResult> errorResponseFunc = result.ErrorCode switch
                {
                    // Ошибки валидации
                    ErrorCode.ValidationFailed =>
                    () =>
                    {
                        if (result.ValidationErrors.Any())
                        {
                            var problemDetails = new ValidationProblemDetails
                            {
                                Title = result.ErrorMessage,
                                Status = StatusCodes.Status400BadRequest,
                                Detail = "Вы не правильно указали данные в полях.",
                            };

                            problemDetails.Errors.Add("ValidationErrors", result.ValidationErrors.ToArray());

                            return BadRequest(problemDetails);
                        }

                        return BadRequest(new ErrorResponse
                        {
                            ErrorCode = result.ErrorCode.ToString()!,
                            Message = result.ErrorMessage ?? _registrationErrorMessageProvider.GetMessage(result.ErrorCode.Value)
                        });
                    }
                    ,
                    //Конфликт данных в бд
                    ErrorCode.LoginAlreadyTaken or ErrorCode.EmailAlreadyRegistered or ErrorCode.PhoneAlreadyRegistered =>
                    () =>
                    {
                        return Conflict(new ErrorResponse
                        {
                            ErrorCode = result.ErrorCode.ToString()!,
                            Message = result.ErrorMessage ?? _registrationErrorMessageProvider.GetMessage(result.ErrorCode.Value),
                        });
                    }
                    ,
                    // Ошибки BadRequest
                    ErrorCode.WeakPassword or ErrorCode.InvalidRole or ErrorCode.InvalidAccountStatus or ErrorCode.DomainCreationError =>
                    () => 
                    {
                        return BadRequest(new ErrorResponse
                        {
                            ErrorCode = result.ErrorCode.ToString()!,
                            Message = result.ErrorMessage ?? _registrationErrorMessageProvider.GetMessage(result.ErrorCode.Value)
                        });
                    }
                    ,
                    // Ошибки, указывающие на проблемы на стороне сервера
                    ErrorCode.SaveUserError or ErrorCode.UnknownError or _ =>
                    () =>
                    {
                        //TODO: Добавить логи
                        return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                        {
                            ErrorCode = result.ErrorCode?.ToString() ?? ErrorCode.UnknownError.ToString(),
                            Message = "Во время регистрации произошла непредвиденная ошибка. Пожалуйста, попробуйте позже."
                        });
                    }
                };
                return errorResponseFunc.Invoke();
            }

            return StatusCode(StatusCodes.Status201Created, result.User);
            //return Created("", result.User);
        }
    }
}
using EnigmaVault.AuthenticationService.Api.DTOs.Requests;
using EnigmaVault.AuthenticationService.Api.DTOs.Responses;
using EnigmaVault.AuthenticationService.Api.Mappers;
using EnigmaVault.AuthenticationService.Application.Abstractions.Providers;
using EnigmaVault.AuthenticationService.Application.Abstractions.UseCases;
using EnigmaVault.AuthenticationService.Application.DTOs;
using EnigmaVault.AuthenticationService.Application.DTOs.Commands;
using EnigmaVault.AuthenticationService.Application.DTOs.Results;
using EnigmaVault.AuthenticationService.Application.Enums;
using EnigmaVault.AuthenticationService.Application.Implementations.Providers;
using Microsoft.AspNetCore.Mvc;

namespace EnigmaVault.AuthenticationService.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IRegisterUserUseCase _registerUserUseCase;
        private readonly IAuthenticateUserUseCase _authenticateUserUseCase;
        private readonly IRecoveryAccessUserUseCase _recoveryAccessUserUseCase;
        private readonly IDefaultErrorMessageProvider _registrationErrorMessageProvider;
        private readonly IDefaultErrorMessageProvider _authenticateErrorMessageProvider;

        public UsersController(IRegisterUserUseCase registerUserUseCase,
                               IAuthenticateUserUseCase authenticateUserUseCase,
                               IRecoveryAccessUserUseCase recoveryAccessUserUseCase,
                               IEnumerable<IDefaultErrorMessageProvider> messagesPrivier)
        {
            _registerUserUseCase = registerUserUseCase;
            _authenticateUserUseCase = authenticateUserUseCase;
            _recoveryAccessUserUseCase = recoveryAccessUserUseCase;

            _registrationErrorMessageProvider = messagesPrivier.OfType<DefaultRegistrationErrorMessageProvider>().FirstOrDefault()!;
            _authenticateErrorMessageProvider = messagesPrivier.OfType<DefaultAuthenticateErrorMessageProvider>().FirstOrDefault()!;
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

            UserResult result = await _registerUserUseCase.RegisterAsync(command);

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

            return StatusCode(StatusCodes.Status201Created);
            //return Created("", result.User);
        }

        /*--Авторизация-----------------------------------------------------------------------------------*/

        //TODO: Добавить JWT токен
        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateUserApiRequest apiRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            UserDto user;
            AuthenticateUserCommand command = apiRequest.ToMapCommand();

            try
            {
                UserResult result = await _authenticateUserUseCase.AuthenticateAsync(command);

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
                                Message = result.ErrorMessage ?? _authenticateErrorMessageProvider.GetMessage(result.ErrorCode.Value)
                            });
                        }
                        ,
                        //Конфликт данных в бд
                        ErrorCode.LoginNotExist =>
                        () =>
                        {
                            return Conflict(new ErrorResponse
                            {
                                ErrorCode = result.ErrorCode.ToString()!,
                                Message = result.ErrorMessage ?? _authenticateErrorMessageProvider.GetMessage(result.ErrorCode.Value),
                            });
                        }
                        ,
                        ErrorCode.InvalidPassword =>
                        () =>
                        {
                            return BadRequest(new ErrorResponse
                            {
                                ErrorCode = result.ErrorCode?.ToString() ?? ErrorCode.UnknownError.ToString(),
                                Message = result.ErrorMessage!
                            });
                        }
                        ,
                        // Ошибки, указывающие на проблемы на стороне сервера
                        ErrorCode.UnknownError or _ =>
                        () =>
                        {
                            //TODO: Добавить логи
                            return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                            {
                                ErrorCode = result.ErrorCode?.ToString() ?? ErrorCode.UnknownError.ToString(),
                                Message = "Во время аутентификации произошла непредвиденная ошибка. Пожалуйста, попробуйте позже."
                            });
                        }
                    };
                    return errorResponseFunc.Invoke();
                }

                user = result.User!;
            }
            catch (Exception)
            {
                //TODO: Добавить логи
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    ErrorCode = ErrorCode.UnknownError.ToString(),
                    Message = "Во время аутентификации произошла непредвиденная ошибка. Пожалуйста, попробуйте позже либо обратитесь в поддержку"
                });
            }

            var userAuthResponse = new UserAuthenticateResponse
            {
                IdUser = user.IdUser,
                Login = user.Login,
                UserName = user.UserName,
                Email = user.Email,
                Phone = user.Phone,
                IdCountry = user.IdCountry,
                IdGender = user.IdGender,
            };

            return StatusCode(StatusCodes.Status200OK, userAuthResponse);
            //return Ok(userAuthResponse);
        }

        /*--Восстановление доступа------------------------------------------------------------------------*/

        [HttpPost("recovery-access")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RecoveryAccess([FromBody] RecoveryAccessUserApiRequest apiRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            RecoveryAccessUserCommand command = apiRequest.ToCommand();

            try
            {
                var result = await _recoveryAccessUserUseCase.RecoveryAccessAsync(command);

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
                                    Detail = "Не правильно указан логин, почта, либо пароль не соответствует мерам безопасности.",
                                };

                                problemDetails.Errors.Add("ValidationErrors", result.ValidationErrors.ToArray());

                                return BadRequest(problemDetails);
                            }

                            return BadRequest(new ErrorResponse
                            {
                                ErrorCode = result.ErrorCode.ToString()!,
                                Message = result.ErrorMessage ?? _authenticateErrorMessageProvider.GetMessage(result.ErrorCode.Value)
                            });
                        }
                        ,
                        //Конфликт данных в бд
                        ErrorCode.LoginNotExist =>
                        () =>
                        {
                            return Conflict(new ErrorResponse
                            {
                                ErrorCode = result.ErrorCode.ToString()!,
                                Message = result.ErrorMessage ?? _authenticateErrorMessageProvider.GetMessage(result.ErrorCode.Value),
                            });
                        }
                        ,
                        ErrorCode.EmailNotExist =>
                        () =>
                        {
                            return Conflict(new ErrorResponse
                            {
                                ErrorCode = result.ErrorCode.ToString()!,
                                Message = result.ErrorMessage ?? _authenticateErrorMessageProvider.GetMessage(result.ErrorCode.Value),
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
                                Message = "Во время восстановление пароля произошла непредвиденная ошибка. Пожалуйста, попробуйте позже."
                            });
                        }
                    };
                    return errorResponseFunc.Invoke();
                }
            }
            catch (Exception)
            {
                //TODO: Добавить логи
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    ErrorCode = ErrorCode.UnknownError.ToString(),
                    Message = "Во время восстановление данных произошла непредвиденная ошибка. Пожалуйста, попробуйте позже либо обратитесь в поддержку"
                });
            }

            return StatusCode(StatusCodes.Status200OK, true);
        }
    }
}
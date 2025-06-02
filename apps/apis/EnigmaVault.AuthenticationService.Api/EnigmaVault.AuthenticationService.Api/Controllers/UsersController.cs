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

        private readonly ILogger<UsersController> _logger;

        public UsersController(IRegisterUserUseCase registerUserUseCase,
                               IAuthenticateUserUseCase authenticateUserUseCase,
                               IRecoveryAccessUserUseCase recoveryAccessUserUseCase,
                               IEnumerable<IDefaultErrorMessageProvider> messagesPrivier,
                               ILogger<UsersController> logger)
        {
            _registerUserUseCase = registerUserUseCase;
            _authenticateUserUseCase = authenticateUserUseCase;
            _recoveryAccessUserUseCase = recoveryAccessUserUseCase;

            _logger = logger;

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
            _logger.LogInformation("<<======Начало регистрации пользователя======>>");

            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState
                   .Where(kvp => kvp.Value!.Errors.Any())
                   .ToDictionary(
                       kvp => kvp.Key,
                       kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                   );

                _logger.LogWarning("Не верный запрос для регистрации. ValidationErrors: {@ValidationErrors}", validationErrors);
                return BadRequest(ModelState);
            }
                
            RegisterUserCommand command = apiRequest.ToMapCommand();

            _logger.LogDebug("Детализация запроса на регистрацию: Login={Login}, Email={Email}, UserName={UserName}, Phone={Phone}, GenderId={IdGender}, CountryId={IdCountry}, PasswordLength={PasswordLength}",
                command.Login, command.Email, command.UserName, command.Phone, command.IdGender, command.IdCountry, command.Password.Length);

            UserResult result = await _registerUserUseCase.RegisterAsync(command);

            _logger.LogDebug("Результат регистрации: Status={Success}, User={User}, ErrorCode={ErrorCode}, ErrorMessage={ErrorMessage}, ValidationErrors={@ValidationErrors}",
                 result.Success, result.User, result.ErrorMessage, result.ErrorMessage, result.ValidationErrors);

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

                            _logger.LogWarning("Ошибки с валидацией при регистрации пользователя {Login}. Status: {Status}, Errors: {@Errors}",
                                command.Login,
                                problemDetails.Status.ToString()!,
                                problemDetails.Errors.ToList());

                            return BadRequest(problemDetails);
                        }

                        _logger.LogWarning("Ошибки с валидацией при регистрации пользователя {Login}. ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}",
                                command.Login,
                                result.ErrorCode.ToString()!,
                                result.ErrorMessage ?? _registrationErrorMessageProvider.GetMessage(result.ErrorCode.Value));

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
                        _logger.LogWarning("Конфликт данных при регистрации пользователя {Login}. ErrorCode: {ErrorCode}, Message: {ErrorMessage}",
                            command.Login,
                            result.ErrorCode.ToString()!,
                            result.ErrorMessage ?? _registrationErrorMessageProvider.GetMessage(result.ErrorCode.Value));

                        return Conflict(new ErrorResponse
                        {
                            ErrorCode = result.ErrorCode.ToString()!,
                            Message = result.ErrorMessage ?? _registrationErrorMessageProvider.GetMessage(result.ErrorCode.Value),
                        });
                    }
                    ,
                    // Ошибки BadRequest
                    ErrorCode.WeakPassword or ErrorCode.InvalidRole or ErrorCode.InvalidAccountStatus =>
                    () => 
                    {
                        _logger.LogWarning("Отклонен запрос на регистрацию для {Login}. Причина: {ErrorCode} - {OriginalErrorMessage}",
                            command.Login,
                            result.ErrorCode.ToString()!,
                            result.ErrorMessage ?? _registrationErrorMessageProvider.GetMessage(result.ErrorCode.Value));

                        return BadRequest(new ErrorResponse
                        {
                            ErrorCode = result.ErrorCode.ToString()!,
                            Message = result.ErrorMessage ?? _registrationErrorMessageProvider.GetMessage(result.ErrorCode.Value)
                        });
                    }
                    ,
                    ErrorCode.DomainCreationError =>
                    () =>
                    {
                        _logger.LogError("Критическая ошибка при создании доменных сущностей для пользователя {Login}, но возвращен BadRequest. ErrorCode: {ErrorCode}, Message: {ErrorMessage}",
                            command.Login,
                            result.ErrorCode.ToString()!,
                            result.ErrorMessage ?? "Ошибка при создании доменных сущностей.");

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
                        _logger.LogError("Ошибка на стороне сервера при регистрации пользователя {Login}. ErrorCode: {ErrorCode}, Message: {ErrorMessage}",
                            command.Login,
                            result.ErrorCode?.ToString() ?? ErrorCode.UnknownError.ToString(),
                            result.ErrorMessage ?? "Во время регистрации произошла непредвиденная ошибка.");

                        return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                        {
                            ErrorCode = result.ErrorCode?.ToString() ?? ErrorCode.UnknownError.ToString(),
                            Message = "Во время регистрации произошла непредвиденная ошибка. Пожалуйста, попробуйте позже."
                        });
                    }
                };
                return errorResponseFunc.Invoke();
            }

            _logger.LogInformation("<<======Регистрация пользователя {UserName} успешно завершена======>>", result.User!.UserName);

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
            _logger.LogInformation("<<======Начало аутентификации пользователя======>>");

            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState
                   .Where(kvp => kvp.Value!.Errors.Any())
                   .ToDictionary(
                       kvp => kvp.Key,
                       kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                   );

                _logger.LogWarning("Не верный запрос для аутентификации. ValidationErrors: {@ValidationErrors}", validationErrors);
                return BadRequest(ModelState);
            }

            UserDto user;
            AuthenticateUserCommand command = apiRequest.ToMapCommand();

            _logger.LogDebug("Детализация запроса на аутентификацию: Login={Login}, PasswordLength={PasswordLength}", command.Login, command.Password.Length);

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

                                _logger.LogWarning("Ошибки с валидацией при аутентификации пользователя {Login}. Status: {Status}, Errors: {@Errors}",
                                    command.Login,
                                    problemDetails.Status.ToString()!,
                                    problemDetails.Errors.ToList());

                                return BadRequest(problemDetails);
                            }

                            _logger.LogWarning("Ошибки с валидацией при аутентификации пользователя {Login}. ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}",
                                    command.Login,
                                    result.ErrorCode.ToString()!,
                                    result.ErrorMessage ?? _registrationErrorMessageProvider.GetMessage(result.ErrorCode.Value));

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
                            _logger.LogWarning("Конфликт данных при аутентификации пользователя {Login}. ErrorCode: {ErrorCode}, Message: {ErrorMessage}",
                                    command.Login,
                                    result.ErrorCode.ToString()!,
                                    result.ErrorMessage ?? _registrationErrorMessageProvider.GetMessage(result.ErrorCode.Value));

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
                            _logger.LogWarning("Конфликт данных при аутентификации пользователя {Login}. ErrorCode: {ErrorCode}, Message: {ErrorMessage}",
                                    command.Login,
                                    result.ErrorCode.ToString()!,
                                    result.ErrorMessage ?? _registrationErrorMessageProvider.GetMessage(result.ErrorCode.Value));

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
                            _logger.LogError("Ошибка на стороне сервера при аутентификации пользователя {Login}. ErrorCode: {ErrorCode}, Message: {ErrorMessage}",
                                    command.Login,
                                    result.ErrorCode?.ToString() ?? ErrorCode.UnknownError.ToString(),
                                    result.ErrorMessage ?? "Во время регистрации произошла непредвиденная ошибка.");

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверки пользователя {Login} в системе.", command.Login);

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

            _logger.LogInformation("<<======Аутентификации пользователя {Login} успешно завершена======>>", userAuthResponse.Login);

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
            _logger.LogInformation("<<======Начало восстановление доступа к учетной записи пользователя======>>");

            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState
                   .Where(kvp => kvp.Value!.Errors.Any())
                   .ToDictionary(
                       kvp => kvp.Key,
                       kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                   );

                _logger.LogWarning("Не верный запрос для восстановление доступа. ValidationErrors: {@ValidationErrors}", validationErrors);
                return BadRequest(ModelState);
            }

            RecoveryAccessUserCommand command = apiRequest.ToCommand();

            _logger.LogDebug("Детализация запроса на восстановление доступа к учетной записи: Login={Login}, Email={Email}, NewPasswordLength={Email}", command.Login, command.Email, command.NewPassword.Length);

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

                                _logger.LogWarning("Ошибки с валидацией при восстановление доступа у пользователя {Login}. Status: {Status}, Errors: {@Errors}",
                                    command.Login,
                                    problemDetails.Status.ToString()!,
                                    problemDetails.Errors.ToList());

                                return BadRequest(problemDetails);
                            }

                            _logger.LogWarning("Ошибки с валидацией при восстановление доступа у пользователя {Login}. ErrorCode: {ErrorCode}, ErrorMessage: {ErrorMessage}",
                                command.Login,
                                result.ErrorCode.ToString()!,
                                result.ErrorMessage ?? _registrationErrorMessageProvider.GetMessage(result.ErrorCode.Value));

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
                            _logger.LogWarning("Конфликт данных при восстановление доступа у пользователя {Login}. ErrorCode: {ErrorCode}, Message: {ErrorMessage}",
                                command.Login,
                                result.ErrorCode.ToString()!,
                                result.ErrorMessage ?? _registrationErrorMessageProvider.GetMessage(result.ErrorCode.Value));

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
                            _logger.LogWarning("Конфликт данных при восстановление доступа у пользователя {Login}. ErrorCode: {ErrorCode}, Message: {ErrorMessage}",
                                command.Login,
                                result.ErrorCode.ToString()!,
                                result.ErrorMessage ?? _registrationErrorMessageProvider.GetMessage(result.ErrorCode.Value));

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
                            _logger.LogError("Ошибка на стороне сервера при восстановление доступа у пользователя {Login}. ErrorCode: {ErrorCode}, Message: {ErrorMessage}",
                                command.Login,
                                result.ErrorCode?.ToString() ?? ErrorCode.UnknownError.ToString(),
                                result.ErrorMessage ?? "Во время восстановление доступа произошла непредвиденная ошибка.");

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при проверки пользователя {Login} в системе.", command.Login);

                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    ErrorCode = ErrorCode.UnknownError.ToString(),
                    Message = "Во время восстановление данных произошла непредвиденная ошибка. Пожалуйста, попробуйте позже либо обратитесь в поддержку"
                });
            }

            _logger.LogInformation("<<======Восстановление доступа у пользователя {Login} успешно завершена======>>", command.Login);
            return StatusCode(StatusCodes.Status200OK, true);
        }
    }
}